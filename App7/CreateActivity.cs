using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App7
{
    [Activity(Label = "Activity1")]
    public class CreateActivity : Activity
    {   
        Button btnUlozit;
        EditText editText;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Create);

            btnUlozit =  FindViewById<Button>(Resource.Id.buttonUlozit);
            editText = FindViewById<EditText>(Resource.Id.editText1);
            btnUlozit.Click += BtnUlozit_Click;        
        }

        private void BtnUlozit_Click(object sender, EventArgs e)
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(imageIntent, "Vyber obrazek"), 0);      
        }
        public static void saveImage(Bitmap bmp)
        {
            try
            {
                using (var os = new System.IO.FileStream(Android.OS.Environment.ExternalStorageDirectory + "/DCIM/Camera/MikeBitMap2.jpg", System.IO.FileMode.CreateNew))
                {
                    bmp.Compress(Bitmap.CompressFormat.Jpeg, 95, os);
                }
            }
            catch (Exception e)
            {
                int a = 10;
            }
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                var result = Steganografie.SchovejText(editText.Text, NGetBitmap(data.Data));
                // saveImage(result);
                SavePictureToDisk("Neco", result);
                var activity = new Intent(this, typeof(MainActivity));             
                StartActivity(activity);
            }
        }
        public void SavePictureToDisk(string filename, Bitmap bmp)
        {
            var dir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);
            var pictures = dir.AbsolutePath;
            //adding a time stamp time file name to allow saving more than one image... otherwise it overwrites the previous saved image of the same name
            string name = filename + System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
            string filePath = System.IO.Path.Combine(pictures, name);
            try
            {
                var os = new System.IO.FileStream(filePath, System.IO.FileMode.CreateNew);
                bmp.Compress(Bitmap.CompressFormat.Png, 100, os);
                
                //mediascan adds the saved image into the gallery         
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }

        }
        private Android.Graphics.Bitmap NGetBitmap(Android.Net.Uri uriImage)
        {
            Android.Graphics.Bitmap mBitmap = null;
            mBitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(this.ContentResolver, uriImage);
            return mBitmap;
        }
    }
}