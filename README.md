# RTLTechAssignmentWR
RTL Tech Assignment WR
First Create a new Database on a MS SQL Server. Name the database RTLFlix
Next execute the DatabaseScript.sql on the newly created database.
Open the api application source code in Visual Studio (preferably version 2017)
Next you have to replace inside the sql connection string the sql server name and possibly add username and password based on the type of authentication setup on your sql server.
For the above step, perform this replacement in each of the copde files inside the "Data" folder in the solution.
The line where you have to replace the sql connection string looks something like:
optionsBuilder.UseSqlServer(@"Data Source=DST19404;Initial Catalog=RTLFlix;Trusted_Connection=True;");
