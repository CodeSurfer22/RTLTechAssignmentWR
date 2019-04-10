using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using RTL.TVMaze.API.Test.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace RTL.TVMaze.API.Test.Controllers
{
    [Route("api/[controller]")]
    public class ScrapingController : Controller
    {
        private ShowContext _Showcontext;
        private CastContext _Castcontext;
        private ShowCastContext _ShowCastcontext;

        /// <summary>
        /// scrapes the TVMaze API for show and cast information;persists the data in storage
        /// </summary>
        /// <returns>Json data.</returns>
        [HttpGet]
        [Route("ScrapeShowsAndPersistDataMain")]
        public async Task<JsonResult> ScrapeShowsAndPersistDataMain()
        {
            var shows = await ScrapeShowsAndPersistData();
            var casts = await PairShowsCastsAndPersistData();
            var result = "Data scraped completed and imported to database.";
            return Json(result);
        }

        /// <summary>
        /// scrapes the TVMaze API for show information;persists the data in storage;  and shows output of scraped data in JSON format
        /// </summary>
        /// <returns>Json data</returns>
        [HttpGet]
        [Route("ScrapeShowsAndPersistData")]
        public async Task<JsonResult> ScrapeShowsAndPersistData()
        {
            _ShowCastcontext = new ShowCastContext();
            _Showcontext = new ShowContext();
            _Castcontext = new CastContext();
            //Clears existing data in the database first:
            _Showcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [Show]");

            string url = "http://api.tvmaze.com/shows";
            List<ShowWithCast> jSonResult = new List<ShowWithCast>();
            try
            {

                _Showcontext = new ShowContext();
                var showName = from sn in _Showcontext.Show
                               select sn;


                using (var client = new HttpClient())
                {
                    using (var r = await client.GetAsync(new Uri(url)))
                    {
                        string JsonStr = await r.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<List<ShowFull>>(JsonStr);

                        foreach (ShowFull fliek in result)
                        {
                            Show aShow = new Show();
                            aShow.ShowID = fliek.id;
                            aShow.ShowName = fliek.name;
                            _Showcontext.Add(aShow);
                            await _Showcontext.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
            }
            var shows = from sctx in _Showcontext.Show
                        select sctx;

            return await Task.FromResult(Json(shows));
        }

        /// <summary>
        /// scrapes the TVMaze API for cast information and pairs it with asociated show;persists the data in storage;  and shows output of scraped data in JSON format
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("PairShowsCastsAndPersistData")]
        public async Task<JsonResult> PairShowsCastsAndPersistData()
        {
            _ShowCastcontext = new ShowCastContext();
            _Castcontext = new CastContext();
            //Deletes all existing data in the local database first
            _ShowCastcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [ShowCast]");
            _Castcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [Cast]");
            string url = "";
            List<ShowWithCast> jSonResult = new List<ShowWithCast>();
            try
            {
                _Castcontext = new CastContext();
                _ShowCastcontext = new ShowCastContext();

                var shows = from sh in _Showcontext.Show
                            select sh;
               
                int ShowCastCounter = 0;
                //loop through all the shows
                foreach (Show s in shows)
                {
                    //retrieves every show's cast
                    //Example - http://api.tvmaze.com/shows/1/cast
                    url = "http://api.tvmaze.com/shows/" + s.ShowID + "/cast";
                    
                    using (var client = new HttpClient())
                    {
                        using (var r = await client.GetAsync(new Uri(url)))
                        {
                            string JsonStr2 = await r.Content.ReadAsStringAsync();
                            var result2 = JsonConvert.DeserializeObject<List<CastFull>>(JsonStr2);

                            foreach (CastFull cast in result2)
                            {
                                ShowCastCounter++;
                                Cast aCast = new Cast();
                                aCast.CastID = cast.person.id;
                                aCast.Name = cast.person.name;
                                aCast.BirthDay = cast.person.birthday;
                                _Castcontext = new CastContext();

                                _Castcontext.Add(aCast);
                                await _Castcontext.SaveChangesAsync();

                                ShowCast cs = new ShowCast();
                                cs.ShowCastID = ShowCastCounter;
                                cs.ShowID = s.ShowID;
                                cs.CastID = aCast.CastID;
                                _ShowCastcontext = new ShowCastContext();
                                _ShowCastcontext.Add(cs);
                                await _ShowCastcontext.SaveChangesAsync();
                            }
                        }
                    }
                }
                await _Castcontext.SaveChangesAsync();
                await _ShowCastcontext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
            }

            _Castcontext = new CastContext();
            var casts = from sh in _Castcontext.Cast
                        select sh;
            return await Task.FromResult(Json(casts));
        }


        /// <summary>
        /// Provides a paginated list of all tv shows containing the id of the TV show and a list of
        ///all the cast that are playing in that TV show.Ordered by birthday descending.
        /// </summary>
        /// <param name="pagingparametermodel">pass in the page number and page size to return a limited set of data.</param>
        /// <returns>Json data</returns>
        [HttpGet]
        [Route("GetPagedListData")]
        public async Task<JsonResult> GetPagedListData([FromQuery]PagingParameterModel pagingparametermodel)
        {
            List<ShowWithCast> jSonResult = new List<ShowWithCast>();
            
            // Get's No of Rows Count   
            int count = jSonResult.Count();

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            int CurrentPage = pagingparametermodel.pageNumber;

            // Parameter is passed from Query string if it is null then it default Value will be pageSize:10  
            int PageSize = pagingparametermodel.pageSize;

            // TotalCount of Records  
            int TotalCount = count;

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            jSonResult = ShowsWithCasts(pagingparametermodel);
            var items = jSonResult;

            // Object which we are going to send in header   
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages
            };

            // Setting Header  
            HttpContext.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return await Task.FromResult(Json(items));
        }

        private List<ShowWithCast> ShowsWithCasts(PagingParameterModel pagingparametermodel)
        {
            List<ShowWithCast> results = new List<ShowWithCast>();
            try
            {
                _ShowCastcontext = new ShowCastContext();
                _Showcontext = new ShowContext();
                _Castcontext = new CastContext();
                var shows = from sh in _Showcontext.Show
                            select sh;
                int ii = shows.Count();
                var showsList = shows.Skip((pagingparametermodel.pageNumber - 1) * pagingparametermodel.pageSize).Take(pagingparametermodel.pageSize).ToList();

                foreach (Show s in showsList)
                {
                    ShowWithCast sc = new ShowWithCast();
                    sc.ShowID = s.ShowID;
                    sc.Name = s.ShowName;

                    var showPairs = from sp in _ShowCastcontext.ShowCast
                                    where sp.ShowID == s.ShowID
                                    select sp;

                    sc.Cast = new List<Cast>();

                    foreach (ShowCast item in showPairs)
                    {
                        var casts = from cst in _Castcontext.Cast
                                    where cst.CastID == item.CastID
                                    select cst;

                        foreach (Cast aCast in casts)
                        {
                            sc.Cast.Add(aCast);
                        }
                    }
                    sc.Cast = sc.Cast.OrderByDescending(x => x.BirthDay).ToList();
                    results.Add(sc);
                }
            }
            catch (Exception ex)
            {
                string er = ex.ToString();
            }
            return results;
        }

        //GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
