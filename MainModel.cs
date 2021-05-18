using OpenCvSharp;
using System;
using System.Text;
using OpenCvSharp.Extensions;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Drawing.Imaging;

namespace Prism_HairStylishUp
{
    /// <summary>
    /// ビジネスモデル
    /// </summary>
    public class MainModel
    {
        /// <summary>
        /// JSONクラス
        /// </summary>
        public class JSON_file
        {
            [JsonProperty("img")]
            public string Img { get; set; }
            [JsonProperty("label")]
            public string Label { get; set; }
        }

        public MainModel()
        {
            
        }
        /// <summary>
        /// カメラクラス
        /// </summary>
        public class Camera
        {
            //static string image_folder = "\\image";
            //string folderPath = App.LocalFilePath + image_folder;
            public VideoCapture ScreenCapture { get; set; }
            

            Mat frame;
            Encode encode;



            //今回は使用しない。
            //String filenameFaceCascade = @"C:\Users\sympo\Documents\RO13\haarcascade_frontalface_alt.xml";
            //CascadeClassifier faceCascade = new CascadeClassifier();

            //コンストラクタ
            public Camera()
            {
                ///カメラはパソコンについているメインカメラを使う0はDefualtのカメラ
                //if (!faceCascade.Load(filenameFaceCascade))
                //{
                //    Console.WriteLine("error");
                //    return;
                //}
                ///クラスのインスタンス生成
                frame = new Mat();
                encode = new Encode();

            }

            public WriteableBitmap Picture { get; set; }　//カメラクラスのプロパティ
            public Mat Temp { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }


            public void CameraOn()
            {              
               ScreenCapture = new VideoCapture(0);
               
               if(ScreenCapture.IsOpened() == false)
                {
                    return;
                }
            }

            /// <summary>
            /// カメラONしたら起動
            /// </summary>
            /// <returns>WriteableBitmap型で返す。</returns>
            public WriteableBitmap Capture()
            {
                ScreenCapture.Read(frame);
                if (frame.Empty())
                    return null;
                Mat resize = frame.Clone(new OpenCvSharp.Rect(80, 0, 480, 480));

                //顔認識
                //detect
                //Rect[] faces = faceCascade.DetectMultiScale(resize);

                //Console.WriteLine(faces.Length);
                //foreach (var item in faces)
                //{
                //    Cv2.Rectangle(resize, item, Scalar.Green); // add rectangle to the image
                //}
                Temp = resize.Clone();



                return resize.ToWriteableBitmap();
            }

            /// <summary>
            /// 撮影ボタンを押した時の処理
            /// </summary>
            /// <returns></returns>
            public string TakePicture()
            {
                //Temp = frame;
                Resize(); //画面スクリーンショット
                //WriteableBitmap型からバイナリーデータを抽出
                var temp = encode.BitmapFromWriteableBitmap(Picture);
                string b64 = encode.BytetoBase64(temp); //BASE64変換

                return b64;
            }

            public void Resize()
            {
                //Mat resize = Temp.Clone(new OpenCvSharp.Rect(80, 0, 480, 480));
                Picture = Temp.ToWriteableBitmap();
                Width = (int)Picture.Width;
                Height = (int)Picture.Height;
            }

            public void Dispose()
            {
                ScreenCapture.Dispose();
            }

            public byte[] Image(string filename)
            {

                string newPath = Path.Combine(Environment.CurrentDirectory, filename);
                Mat image = Cv2.ImRead(newPath, ImreadModes.Color);
                var bytes = image.ToBytes();

                var temp = Encoding.UTF8.GetString(bytes);
                string savePath = App.LocalFilePath + "test.txt";

                System.IO.File.WriteAllText(savePath, temp, Encoding.Default);
                // Marshal.Copy(image.Data, bytes, 0, bytes.Length);
                return bytes;
            }

        }

        /// <summary>
        /// エンコードクラス
        /// </summary>
        public class Encode
        {
            int num;
            public Encode() { num = 0; }

            public byte[] BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
            {
                
                //メモリストリームを使ってBitmapエンコーダーを使ってバイナリーコードを抽出
                using (MemoryStream outStream = new MemoryStream())
                {
                    JpegBitmapEncoder enc = new JpegBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeBmp));
                    enc.Save(outStream);
                    byte[] temp = outStream.ToArray();
                    return temp;
                }

            }

            //public string GetImage(string path, int width, int height)
            //{
            //    MemoryStream stream = new MemoryStream();
            //    string newPath = Path.Combine(Environment.CurrentDirectory, path);

            //    Image img = Image.FromFile(newPath);
            //    img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

            //    byte[] bytes = stream.ToArray();
            //    //var temp = Encoding.UTF8.GetString(bytes);
            //    //var temp_byte = Encoding.UTF8.GetBytes(temp);

            //    string mpic = BytetoBase64(bytes);

            //    return mpic;
            //}

            //Convertライブラリを使ってBASE64文字列に変換
            public string BytetoBase64(byte[] bt)
            {
                string temp = Convert.ToBase64String(bt);

                return temp;
            }

            //BASE64を文字列貰ってバイナリー化しLOCALに保存してBitmapに変換する。
            public BitmapImage Base64toImage(string base64, string filepath)
            {
                var temp = Convert.FromBase64String(base64);
                
                

                FileInfo fileInfo = new FileInfo(filepath);

                if (fileInfo.Exists)
                {
                    var filename = System.IO.Path.GetFileNameWithoutExtension(filepath);
                    filename += num;
                    num++;
                    var directory = System.IO.Path.GetDirectoryName(filepath);

                    filepath = System.IO.Path.Combine(directory, filename + ".bmp");

                    FileStream fp = new FileStream(filepath, FileMode.Create);
                    fp.Write(temp, 0, temp.Length);
                    fp.Close();
                }
                else
                {
                    FileStream fp = new FileStream(filepath, FileMode.Create);
                    fp.Write(temp, 0, temp.Length);
                    fp.Close();
                }

                var bitmap = new BitmapImage(new Uri(filepath));
                
                return bitmap;

            }

            public BitmapImage Base64toImage(string base64)
            {
                ImageFormat format = ImageFormat.Bmp;

                var temp = Convert.FromBase64String(base64);

                MemoryStream ms = new MemoryStream(temp);

                //ms.Write(temp, 0, temp.Length);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                ms.Close();
                return bitmap;
            }

        }

        /// <summary>
        /// Protocolクラス
        /// </summary>
        public class Protocol
        {
            #region プロパティ
            public string pro_Get_Data { get; private set; }
            public string pro_Post_Data { get; private set; }

            public JSON_file Json { get; private set; }

            public BitmapImage pro_Convert_Bmp { get; private set; }
            #endregion

            Encode protocol_encode;

            readonly string URL = "http://127.0.0.1:50000";
            //WebResponse response;
            //WebRequest request = WebRequest.Create(URL + "/echo_back");

            public Protocol()
            {
                protocol_encode = new Encode();
            }

            //WebRequestのライブラリでGETリクエスト要請
            //public void GetRequest()
            //{
            //    //WebRequest request = WebRequest.Create(URL); // 호출할 url
            //    request.Method = "GET";

            //    WebResponse response = request.GetResponse();
            //    Stream dataStream = response.GetResponseStream();
            //    StreamReader reader = new StreamReader(dataStream);

            //    string responseFromServer = reader.ReadToEnd();

            //    this.pro_Get_Data = responseFromServer;

            //    reader.Close();
            //    dataStream.Close();
            //    response.Close();
            //}

            //PostRequestのライブラリでPOSTリクエスト要請
            public void PostRequest(string b64, string flg)
            {

                Json = new JSON_file
                {
                    Img = b64,
                    Label = flg
                };

                var json_seri = JsonConvert.SerializeObject(Json);

                pro_Post_Data = json_seri;

                Byte[] bytes = Encoding.ASCII.GetBytes(json_seri);

                // Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create(URL + "/echo_back");
                // Set the Method property of the request to POST.
                request.Method = "POST";
                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/json";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = bytes.Length;
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(bytes, 0, bytes.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                pro_Get_Data = reader.ReadToEnd();

                //貰ったJSON文字列をデシリアライズする。
                Json = JsonConvert.DeserializeObject<JSON_file>(pro_Get_Data);

                pro_Convert_Bmp = protocol_encode.Base64toImage(Json.Img);

                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
            }
        }
    }
}
