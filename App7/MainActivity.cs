using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Graphics;
using Android.Net;

namespace App7
{
    [Activity(Label = "Steganografie", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Button btn;
        Button btnPotvrdit;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            btn = FindViewById<Button>(Resource.Id.button1);  // dešifruj       
            btnPotvrdit = FindViewById<Button>(Resource.Id.btnPotvrdit); // zašifruj         

         //   btnPotvrdit.Clickable = false; // tlacitko nebude reagovat na kliknuti
            // Eventy
            btnPotvrdit.Click += BtnPotvrdit_Click;
            btn.Click += Btn_Click;
        }

        private void BtnPotvrdit_Click(object sender, System.EventArgs e)
        {
            var activity = new Intent(this, typeof(CreateActivity));
            StartActivity(activity);
        }

        private void Btn_Click(object sender, System.EventArgs e)
        {
            var activity = new Intent(this, typeof(DesifrujActivity));
            StartActivity(activity);
        }     
        
    }
}

