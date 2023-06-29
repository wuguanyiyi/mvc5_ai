using About.Data;
using About.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;

namespace About.Controllers
{
    public class EmbedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly AccountContext _ctx;

        public EmbedController(AccountContext ctx)
        {
            _ctx = ctx;
        }

        public class MyJsonObject
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }

            [JsonPropertyName("embedding")]
            public List<float> Embedding { get; set; }
        }



        public async Task<IActionResult> readfile()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "files");

            string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");

            int idnum = 1;
            



            foreach (string filePath in jsonFiles)
            {
                string id = $"E{idnum:D4}";
                string jsonContent = System.IO.File.ReadAllText(filePath);

                
                MyJsonObject obj = JsonConvert.DeserializeObject<MyJsonObject>(jsonContent);

                // 現在你可以使用 obj.Context 和 obj.Embedding 存取內容了
                Console.WriteLine(obj.Content);
                Console.WriteLine(obj.Embedding);


                string context = obj.Content;
                string embedding = System.Text.Json.JsonSerializer.Serialize(obj.Embedding);


                Embedding embs = new Embedding()
                {
                    EmbeddingID = id,
                    EmbeddingQuestion = null,
                    EmbeddingAnswer = null,
                    QA = context,
                    EmbeddingVectors = embedding
                };

                _ctx.Add(embs);
                await _ctx.SaveChangesAsync();
                idnum++;
            }


            return View();
        }
    }
}

