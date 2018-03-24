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
    internal class LoginDialog : DialogFragment
    {
        private ProgressBar loadingCircle;

        public event EventHandler<string> OnUserLoggedIn;

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var view = LayoutInflater.From(App.CurrentActivity).Inflate(Resource.Layout.login_dialog, null, false);
            var emailTb = view.FindViewById<EditText>(Resource.Id.emailTb);
            var passwordTb = view.FindViewById<EditText>(Resource.Id.passwordTb);
            var loginBtn = view.FindViewById<Button>(Resource.Id.loginBtn);
            loadingCircle = view.FindViewById<ProgressBar>(Resource.Id.loadingCircle);

            loginBtn.Click += async delegate
            {
                using (var validator = new Validator())
                {
                    loadingCircle.Visibility = ViewStates.Visible;
                    loginBtn.Enabled = false;

                    validator.ValidateIsNotEmpty(emailTb, true);
                    validator.ValidateIsNotEmpty(passwordTb, true);

                    if (validator.PassedValidation)
                    {
                        var data = new LoginData { Email = emailTb.Text.Trim(), Password = passwordTb.Text.Trim() };
                        await LoginUser(data);
                    }

                    loginBtn.Enabled = true;
                    loadingCircle.Visibility = ViewStates.Gone;
                }
            };

            return new AlertDialog.Builder(Activity)
                .SetTitle("Login")
                .SetView(view)
                .Create();
        }

        private async Task LoginUser(LoginData data)
        {
            var response = await IdeaBagApi.Instance.LoginAsync(data);

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
        }
    }
}