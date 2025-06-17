namespace HandHubAPI;

public static class Enviroments
{
    public static string ConnectionString_SSMS = @"Server=localhost\SQLEXPRESS02; Initial Catalog=HandHub; Integrated Security=True; TrustServerCertificate=True;";
    public static string ConnectionString_Docker = "Server=localhost, 1433; Database=HandHub;User Id=sa;Password=DinhKhacDien1009!@#;TrustServerCertificate=True;";
}