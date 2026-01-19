namespace DataLayer;


using Microsoft.Extensions.Configuration;

/*
    * DataSource => SQLITE | SQLSERVER {EITHER CASE IT IS A FILE}
    * DbName => FlashCards
    * 3 Tables {Card / Stck / Study} -- Many Cards to 1 Stack Relation 
    * 1 VIEW which is virtual table of Stack and Card Data mostly for viewing
    * Scripts for the following:
    * Creation of all tables. All will be blank except Stack which record 1 is DEFAULT
    * Reading the data / Update Data / Deleting Data              
 */
public class DbSetup
{
    public DbSetup()
    {

    }
}
