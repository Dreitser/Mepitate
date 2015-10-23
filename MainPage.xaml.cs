using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

//Sound
using Windows.Media.Play;

namespace Mepitate
{
    public sealed partial class MainPage : Page
    {

        //PIR Motion Detector variables
        private const int PIR_PIN = 16;
        private GpioPin PinPIR;

        /// <summary>
        /// Entry point of the application
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            InitializeGPIO();

            // At this point, the application waits for motion to be detected by
            // the PIR sensor, which then calls the PinPIR_ValueChanged() fucntion
        }

        #region GPIO code (PIR)

        /// <summary>
        /// Initialize the GPIO ports on the Raspberry Pi
        /// 
        /// GPIO PIN 16 = PIR Signal
        /// </summary>
        private void InitializeGPIO()
        {
            try
            {
                //Obtain a reference to the GPIO Controller
                var gpio = GpioController.GetDefault();

                // Show an error if there is no GPIO controller
                if (gpio == null)
                {
                    PinLED = null;
                    GpioStatus.Text = "No GPIO controller found on this device.";
                    return;
                }

                //Open the GPIO port for LED
                PinLED = gpio.OpenPin(LED_PIN);

                //set the mode as Output (we are WRITING a signal to this port)
                PinLED.SetDriveMode(GpioPinDriveMode.Output);

                //Open the GPIO port for PIR motion sensor
                PinPIR = gpio.OpenPin(PIR_PIN);

                //PIR motion sensor - Ignore changes in value of less than 50ms
                PinPIR.DebounceTimeout = new TimeSpan(0, 0, 0, 0, 50);

                //set the mode as Input (we are READING a signal from this port)
                PinPIR.SetDriveMode(GpioPinDriveMode.Input);

                //wire the ValueChanged event to the PinPIR_ValueChanged() function
                //when this value changes (motion is detected), the function is called
                PinPIR.ValueChanged += PinPIR_ValueChanged;

                GpioStatus.Text = "GPIO pins " + PIR_PIN.ToString() + " initialized correctly.";
            }
            catch (Exception ex)
            {
                GpioStatus.Text = "GPIO init error: " + ex.Message;
            }

        }

        /// <summary>
        /// Event called when GPIO PIN 16 changes (PIR signal)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void PinPIR_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            //simple guard to prevent it from triggering this function again before it's compelted the first time - one sound at a time please
            if (IsPlaying)
                return;
            else
                IsPlaying = true;
            try
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    PIRStatus.Text = "New PIR pin value: " + args.Edge.ToString();
                    //To Do
                    //Play.Sound;
                    
                   });
            }
            catch (Exception ex)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    PIRStatus.Text = "PIR Error: " + ex.Message;
                });
            }
            finally
            {
                isPlaying = false;
            }

            return;
        }

       
        #endregion


    }
