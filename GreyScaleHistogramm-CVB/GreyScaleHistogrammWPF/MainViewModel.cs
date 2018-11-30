using Stemmer.Cvb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stemmer.Cvb.Driver;
using Stemmer.Cvb.Wpf;
using System.Windows;
using System.IO;
using Stemmer.Cvb.Async;
using Stemmer.Cvb.Utilities;

namespace RGBHistogrammWPF
{
  /// <summary>
  /// The MainWindow of the Application
  /// </summary>
  public class MainViewModel : ViewModelBase, INotifyPropertyChanged
  {
    /// <summary>
    /// Calculates the Histogramm and 
    /// sets the button property 
    /// </summary>
    public MainViewModel()
    {
      Calculate = new DelegateAction(CalculateHistogramm);
      OpenButton = new DelegateAction(OpenButtonClicked);
      GrabCheckBoxCommand = new DelegateAction(CheckBoxCheckedChange);

      Image img = Image.FromFile(@"C:\Users\jsiedersberger\Pictures\Saved Pictures\lamborghini.jpg"); //Default Image
      
      Image = PixelKernel.Calculate(img);
      CalculateHistogramm();
    }


    public ICommand Calculate { get; set; }

    private void CalculateHistogramm()
    {
      var elapsedTime = Measure(() =>
      {
        Data = Histogram.Create(Image);
      });

      Time = $"{elapsedTime.TotalMilliseconds}ms";
    }

    private static TimeSpan Measure(Action action)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      action();
      stopwatch.Stop();
      return TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);
    }

    /// <summary>
    /// The image to display.
    /// </summary>
    public Image Image
    {
      get => _img;
      private set
      {
        _img = value;
        CalculateHistogramm();
        FirePropertyChanged();
      }
    }
    private Image _img;

    /// <summary>
    /// Contains how often a color value appeard in the image
    /// </summary>
    public int[][] Data
    {
      get
      {
        return _test;
      }
      private set
      {
        _test = value;
        FirePropertyChanged();
      }
    }
    private int[][] _test;

    /// <summary>
    /// The duration of the histogramm calculation
    /// </summary>
    public string Time
    {
      get => _time;
      set
      {
        _time = value;
        FirePropertyChanged();
      }
    }
    private string _time;

    public ICommand OpenButton { get; set; }

    private bool _isOpenButtonEnabled = true;

    private void OpenButtonClicked()
    {
      if (_isOpenButtonEnabled == false)
        return;
      try
      {
        Device device = FileDialogs.LoadByDialog(path => DeviceFactory.Open(path), "CVB Devices|*.vin;*.emu;*.avi");
        if (device == null)
          return; // canceled

        Device = device;
      }
      catch (IOException ex)
      {
        MessageBox.Show(ex.Message, "Error loading device", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private bool _isGrabCheckBoxChecked;
    public bool GrabCheckBox
    {
      get => _isGrabCheckBoxChecked;

      set
      {
        if (_isGrabCheckBoxChecked != value)
        {
          _isGrabCheckBoxChecked = value;
          FirePropertyChanged();
        }
      }
    }

    public ICommand GrabCheckBoxCommand { get; set; }

    private async void CheckBoxCheckedChange()
    {
      if (GrabCheckBox)
      {
        Device.Stream.Start();
        _isOpenButtonEnabled = false;
        try
        {
          while (GrabCheckBox)
          {
            StreamImage image = await Device.Stream.WaitAsync();

            //Image = image;           
            CalculateHistogramm();
          }
        }
        catch (OperationCanceledException)
        {
          // acquisition was aborted
        }
        finally
        {
          _isOpenButtonEnabled = true;
        }
      }
      else
      {
        if (Device.Stream.IsRunning)
          Device.Stream.Abort();
      }
    }

    private Device Device
    {
      get { return _device; }
      set
      {
        if (!ReferenceEquals(value, _device))
        {
          Image = value?.DeviceImage;

          _device?.Dispose(); // old one is not needed anymore
          _device = value;
        }
      }
    }
    private Device _device;

  }
}
