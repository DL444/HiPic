using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
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

        public string GetPicJsonString(string keyword, int mime = 0, int pages = 0)
        {
            string result = "";

            StringBuilder builder = new StringBuilder();
            builder.Append(apiUrl);
            builder.Append("?");
            builder.AppendFormat("keyword={0}", keyword);
            builder.AppendFormat("&mime={0}", mime.ToString());
            builder.AppendFormat("&pages={0}", pages.ToString());

            HttpWebRequest req = WebRequest.Create(builder.ToString()) as HttpWebRequest;

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            try
            {
                //获取内容
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            finally
            {
                stream.Close();
            }
            return result;
        }

        public List<string> GetImages(string keyword)
        {
            List<string> images = new List<string>();
            string json = GetPicJsonString(keyword);

            PicJson picJson = JsonConvert.DeserializeObject<PicJson>(json);

            List<PicData> picDatas = picJson.data.list;
            foreach (PicData data in picDatas)
            {
                images.Add(data.image_url);
            }
            return images;
        }
    }
}
