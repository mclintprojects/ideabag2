using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Helpers;
using Newtonsoft.Json;
using ProgrammingIdeas.Api;
using ProgrammingIdeas.Helpers;
using ProgrammingIdeas.Models;
using System;
using System.Threading.Tasks;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using DialogFragment = Android.Support.V4.App.DialogFragment;

namespace ProgrammingIdeas.Fragments
{
    internal class RegisterDialog : DialogFragment
    {
        private ProgressBar loadingCircle;

        public event EventHandler<string> OnUserLoggedIn;

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var view = LayoutInflater.From(App.CurrentActivity).Inflate(Resource.Layout.register_dialog, null, false);
            var emailTb = view.FindViewById<EditText>(Resource.Id.emailTb);
            var passwordTb = view.FindViewById<EditText>(Resource.Id.passwordTb);
            var retypePasswordTb = view.FindViewById<EditText>(Resource.Id.retypePasswordTb);
            var loginBtn = view.FindViewById<Button>(Resource.Id.loginBtn);
            loadingCircle = view.FindViewById<ProgressBar>(Resource.Id.loadingCircle);

            loginBtn.Click += delegate
            {
                using (var validator = new Validator())
                {
                    loadingCircle.Visibility = ViewStates.Visible;
                    validator.ValidateIsNotEmpty(emailTb, true);
                    validator.ValidateIsSame(passwordTb, retypePasswordTb);

                    if (validator.PassedValidation)
                    {
                        var data = new LoginData { Email = emailTb.Text.Trim(), Password = passwordTb.Text.Trim() };
                        RegisterUser(data);
                    }
                }
            };

            return new AlertDialog.Builder(Activity)
                .SetTitle("Sign up")
                .SetView(view)
                .Create();
        }

        private async Task RegisterUser(LoginData data)
        {
            var response = await IdeaBagApi.Instance.RegisterAsync(data);

            if (response.Payload != null)
            {
                Global.AuthUser(response.Payload);

                var expirationDate = DateTime.Now.AddMilliseconds(response.Payload.ExpiresInMillis);
                PreferenceManager.Instance.AddEntry("expiresIn", JsonConvert.SerializeObject(expirationDate));

                OnUserLoggedIn?.Invoke(this, response.Payload.Email);
                Dismiss();
            }
            else
                Snackbar.Make(loadingCircle, response.ErrorMessage, Snackbar.LengthLong).Show();

            loadingCircle.Visibility = ViewStates.Gone;
        }
    }
}