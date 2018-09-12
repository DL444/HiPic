using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HiPic
{
#pragma warning disable CS0649

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

#pragma warning restore CS0649

    static class PicFinder
    {
        public static async Task<string> GetPicJsonString(string apiRoot, string keyword, int mime = 0, int pages = 0)
        {
            string url = $"{apiRoot}?keyword={keyword}&mime={mime}&pages={pages}";
            HttpWebRequest req = WebRequest.CreateHttp(url);

            HttpWebResponse resp = (HttpWebResponse)await req.GetResponseAsync();
            Stream stream = resp.GetResponseStream();
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static async Task<List<string>> GetImages(string apiRoot, string keyword)
        {
            List<string> images = new List<string>();
            string json = await GetPicJsonString(apiRoot, keyword);

            PicJson picJson = JsonConvert.DeserializeObject<PicJson>(json);

            foreach (PicData data in picJson.data.list)
            {
                images.Add(data.image_url);
            }
            return images;
        }
    }
}
