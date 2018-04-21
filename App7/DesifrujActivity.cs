using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App7
{
    [Activity(Label = "DesifrujActivity")]
    public class DesifrujActivity : Activity
    {
        Button btnDesifruj;
        Button btnZpet;
        EditText text;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Desifruj);

            btnDesifruj = FindViewById<Button>(Resource.Id.btnDesifruj);
            btnZpet = FindViewById<Button>(Resource.Id.btnZpet);
            text = FindViewById<EditText>(Resource.Id.editTextDesifruj);

            btnDesifruj.Click += BtnDesifruj_Click;
            btnZpet.Click += BtnZpet_Click;
            
        }

        private void BtnZpet_Click(object sender, EventArgs e)
        {
            var activity = new Intent(this, typeof(MainActivity));
            StartActivity(activity);
        }

        private void BtnDesifruj_Click(object sender, EventArgs e)
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(imageIntent, "Vyber obrazek"), 0);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
               text.Text =  Steganografie.ZiskejText(NGetBitmap(data.Data));            
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