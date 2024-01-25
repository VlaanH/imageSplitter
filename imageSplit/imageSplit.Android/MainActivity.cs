using Android.App;
using Android.Content.PM;
using Android.Media;

using Android.OS;
using System.IO;
using System.Threading;


namespace imageSplit.Android
{
    [Activity(Label = "imageSplitter", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Media.StartScanForMedia();
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());
           
        }
       
    }
    

    public class Media
    {
        static void CrateSplitDir()
        {
            if (Directory.Exists(StaticData.documentsPath)==false)
            {
                Directory.CreateDirectory(StaticData.documentsPath);
            }
        }

        
        public static void StartScanForMedia()
        {
            new Thread(()=>
            {
                CrateSplitDir();
               
                Directory.CreateDirectory(StaticData.documentsPath);
                
                while (true)
                {
                    foreach (var file in Directory.GetFiles(StaticData.documentsPath))
                    {
                        
                        Thread.Sleep(1200);
                        
                        MediaScannerConnection.ScanFile(Application.Context, new string[] { file }, null, null);
                    }
                  
                }
            }).Start();
        }
    }
    
   
}