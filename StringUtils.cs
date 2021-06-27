namespace MonitorNotas
{
    public static class Utils
    {
        public static string LimparNota(this string nota)
        {
            string notaLimpa = nota.Replace('\n', ' ')
                                   .Replace('\r', ' ')
                                   .Replace("&atilde;","ã")
                                   .Replace("&ccedil;","ç");
            return notaLimpa.Trim();
        }
    }
}
