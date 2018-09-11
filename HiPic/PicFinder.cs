using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HiPic
{
    class PicData
    {
        public int out_id;
        public string image_url;
    }
    class PicJsonData
    {
        public List<PicData> list;
        public bool more;
    }
    class PicJson
    {
        public int status;
        public PicJsonData data;
    }
    class PicFinder
    {
        readonly string apiUrl = "https://www.doutula.com/api/search";

        public PicFinder() { }

        public async Task<string> GetPicJsonString(string keyword, int mime = 0, int pages = 0)
        {
            string result = "";

            StringBuilder builder = new StringBuilder();
            builder.Append(apiUrl);
            builder.Append("?");
            builder.Append($"keyword={keyword}");
            builder.Append($"&mime={mime}");
            builder.Append($"&pages={pages}");

            HttpWebRequest req = WebRequest.Create(builder.ToString()) as HttpWebRequest;

            HttpWebResponse resp = (HttpWebResponse)await req.GetResponseAsync();
            Stream stream = resp.GetResponseStream();
            //try
            //{
                //获取内容
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            //}
            //finally
            //{
            //    stream.Close();
            //}
            // StreamReader.Close() already closes the underlying stream. 
            return result;
        }

        public async Task<List<string>> GetImages(string keyword)
        {
            List<string> images = new List<string>();
            string json = await GetPicJsonString(keyword);

            PicJson picJson = JsonConvert.DeserializeObject<PicJson>(json);

            foreach (PicData data in picJson.data.list)
            {
                images.Add(data.image_url);
            }
            return images;
        }
    }
}
