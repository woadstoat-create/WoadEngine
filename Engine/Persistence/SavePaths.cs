namespace WoadEngine.Persistence
{
    public static class SavePaths
    {
        public static string PerGameFile(string companyOrNamespace, string gameName, string fileName)
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(root, companyOrNamespace, gameName, fileName);
        }
    }
}