using System;
using System.Threading;
using System.Threading.Tasks;
using imageSplit.ImgSplitter;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Xamarin.Forms;


namespace imageSplit
{
    public static class StaticData
    {
        public static string documentsPath = "/storage/emulated/0/Download/split/";
    }

    public partial class MainPage : ContentPage
    {
        async Task<FileData> PickAndShow()
        {
            imageSplit.App.ClearCurrent();
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                    return null; // user canceled file picking
               
                string fileName = fileData.FilePath;
                string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);

                Console.WriteLine("File name chosen: " + fileName);
                Console.WriteLine("File data: " + contents);

                SizeLabel.Text = "File Size:"+fileData.DataArray.Length.ToString()+" | Name:"+ fileData.FileName;
               
                
                return fileData;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception choosing file: " + ex.ToString());
            }

            return null;
        }

        
        public MainPage()
        {
            InitializeComponent();
        }

        private static SkiaSharp.SKBitmap selectedBitmap;
        private static int selectedValue = 0;
        private string imgName;

        private async void PickImge_OnClicked(object sender, EventArgs e)
        {
            var imgData = await PickAndShow();
            
            var valueSlider = ((int)SliderSplit.Value);

            selectedValue = valueSlider;
            selectedBitmap = ImgSplitter.Split.ConvertFileDataToSKBitmap(imgData);

            var sourceImage = ImgSplitter.Split.DrawStripesOnImage(selectedBitmap,valueSlider,0);
            
            ImageViw.Source = sourceImage;
            
            imgName = imgData.FileName;
        }

        private async void Slider_OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            var valueSlider = ((int)SliderSplit.Value);
            
         
            
            SliderLabel.Text = valueSlider.ToString();

            if (selectedValue!=valueSlider)
            {
                if (selectedBitmap!=null)
                {
                    selectedValue = valueSlider;

                    ImageSource source = null;

                    new Thread(() =>
                    {
                        source = ImgSplitter.Split.DrawStripesOnImage(selectedBitmap,valueSlider,0);
                        
                    }).Start();

                    //button lock
                    Save.IsEnabled = false;
                    PickImge.IsEnabled = false;
                    
                  
                    
                    while (source==null)
                    {
                       await Task.Delay(600);
                    }
                    
                   

                    //image fading and setting
                    await ImageViw.FadeTo(0, 200);
                    ImageViw.Source = source;
                    await ImageViw.FadeTo(1, 200);

                
                
                    
                    //unlocking buttons
                    Save.IsEnabled = true;
                    PickImge.IsEnabled = true;

                }
            }
            
            
        }

        private async void ButtonSave_OnClicked(object sender, EventArgs e)
        {
            if (selectedBitmap!=null)
            {
                var valueSlider = ((int)SliderSplit.Value);
            
                await DisplayAlert("Path", StaticData.documentsPath, "OK");
            
                Split.SplitAndSaveBitmap(selectedBitmap, valueSlider, StaticData.documentsPath, imgName);
            }
            else
            {
                await DisplayAlert("Error", "Image not selected", "OK");
            }
            
        }
    }
}