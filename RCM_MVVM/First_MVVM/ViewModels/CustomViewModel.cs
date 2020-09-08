using Prism.Mvvm;

namespace First_MVVM.ViewModels 
{
    public class CustomViewModel : BindableBase
    {
        private string _title = "CustomViewModel Title";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        
        public CustomViewModel()
        {

        }
        
    }
}
