using HtmlAgilityPack;
using ScrapySharp.Html.Forms;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonitorNotas
{
    class Crawler
    {
        private string _usuario { get; set; }
        private string _senha { get; set; }
        private ScrapingBrowser _browser = new ScrapingBrowser();

        public Crawler(string usuario, string senha)
        {
            this._usuario = usuario;
            this._senha = senha;
        }

        public List<Materia> PegarNotas()
        {
            List<Materia> materias = new List<Materia>();

            WebPage pageResult = this._browser.NavigateToPage(new Uri("http://san.fatecsp.br/"));

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();

            PageWebForm form = pageResult.FindFormById("login");

            if(form != null)
            {
            form["userid"] = this._usuario;
            form["password"] = this._senha;

            form.Method = HttpVerb.Post;
            pageResult = form.Submit();
            }

            pageResult = this._browser.NavigateToPage(new Uri("http://san.fatecsp.br/index.php?task=conceitos_finais"));

            if (pageResult.Html.InnerHtml.Contains("<input type=\"password\" name=\"password\" id=\"password\">"))
                throw new LoginException();

            doc.LoadHtml(pageResult.Html.InnerHtml);

            List<HtmlNode> trNodes = doc.GetElementbyId("disciplinas").Descendants("tr").Where(n => !string.IsNullOrWhiteSpace(n.InnerText)
                                                                                                && !n.InnerText.Contains("Nome da Disciplina")
                                                                                                && !n.InnerHtml.Contains("<td align=\"left\"><font size=\"1\">")).ToList();

            foreach (HtmlNode node in trNodes)
            {
                Materia materia = new Materia();
                materia.Nome = node.SelectSingleNode("td[2]").InnerText.Trim();
                materia.Nota = node.SelectSingleNode("td[5]").InnerText.Replace("&atilde;", "ã")
                                                                       .Replace("&ccedil;", "ç")
                                                                       .Replace("\n", "")
                                                                       .Replace("\n", "")
                                                                       .Trim();
                materias.Add(materia);
            }

            return materias;
        }
    }
}
