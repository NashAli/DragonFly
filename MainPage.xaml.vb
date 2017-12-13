' DragonFly


Imports System.Net
Imports Windows.Devices.Bluetooth
Imports Windows.Services.Maps
Imports Windows.Devices.Radios
Imports Windows.Devices.Usb
Imports Windows.Devices.Enumeration
Imports Windows.Devices.Geolocation
Imports Windows.Devices.Gpio
Imports Windows.Devices.I2c
Imports Windows.Devices.Spi
Imports Windows.Devices.WiFi
Imports Windows.Media.SpeechRecognition
Imports Windows.Media.SpeechSynthesis
Imports Windows.Networking
Imports Windows.Networking.Connectivity
Imports Windows.Security.Credentials
Imports Windows.Security.ExchangeActiveSyncProvisioning
Imports Windows.Storage
Imports Windows.Storage.Search
Imports Windows.Storage.Streams
Imports Windows.System
Imports Windows.System.Profile
Imports Windows.UI.Core
Imports Windows.ApplicationModel.AppService
Imports System.Collections.ObjectModel
Imports Windows.Devices.Bluetooth.Rfcomm
Imports Windows.Phone
Imports System.IO
Imports System.Collections.Generic
Imports System
Imports Windows.UI.Xaml.Controls
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page


    '   Gyro stuff

    Partial Public Class MPU6050
        Implements IDisposable

        Public Sub Dispose() Implements IDisposable.Dispose
            Throw New NotImplementedException()
        End Sub
    End Class

    Public Class MpuSensorValue
        Public Property AccelerationX() As Single
            Get
                Return m_AccelerationX
            End Get
            Set
                m_AccelerationX = Value
            End Set
        End Property
        Private m_AccelerationX As Single
        Public Property AccelerationY() As Single
            Get
                Return m_AccelerationY
            End Get
            Set
                m_AccelerationY = Value
            End Set
        End Property
        Private m_AccelerationY As Single
        Public Property AccelerationZ() As Single
            Get
                Return m_AccelerationZ
            End Get
            Set
                m_AccelerationZ = Value
            End Set
        End Property
        Private m_AccelerationZ As Single
        Public Property GyroX() As Single
            Get
                Return m_GyroX
            End Get
            Set
                m_GyroX = Value
            End Set
        End Property
        Private m_GyroX As Single
        Public Property GyroY() As Single
            Get
                Return m_GyroY
            End Get
            Set
                m_GyroY = Value
            End Set
        End Property
        Private m_GyroY As Single
        Public Property GyroZ() As Single
            Get
                Return m_GyroZ
            End Get
            Set
                m_GyroZ = Value
            End Set
        End Property
        Private m_GyroZ As Single
    End Class
    Public Class MpuSensorEventArgs
        Inherits EventArgs
        Public Property Status() As Byte
            Get
                Return m_Status
            End Get
            Set
                m_Status = Value
            End Set
        End Property
        Private m_Status As Byte
        Public Property SamplePeriod() As Single
            Get
                Return m_SamplePeriod
            End Get
            Set
                m_SamplePeriod = Value
            End Set
        End Property
        Private m_SamplePeriod As Single
        Public Property Values() As MpuSensorValue()
            Get
                Return m_Values
            End Get
            Set
                m_Values = Value
            End Set
        End Property
        Private m_Values As MpuSensorValue()
    End Class


    Private Const CONFIG As Byte = &H1A    '   bit 3 & 4 select the full range scale
    Private Const GYRO_CONFIG As Byte = &H1B    '   bit 3 & 4 select the full range scale
    Private Const ACCEL_CONFIG As Byte = &H1C    '   bit 3 & 4 select the full range scale
    Public Const ADDRESS As Byte = &H68
    Private Const SMPLRT_DIV As Byte = &H19
    Private Const FIFO_EN As Byte = &H23
    Private Const INT_ENABLE As Byte = &H38
    Private Const INT_STATUS As Byte = &H3A
    Private Const USER_CTRL As Byte = &H6A
    Private Const FIFO_COUNT As Byte = &H72
    Private Const FIFO_R_W As Byte = &H74
    Private Const WHO_AM_I As Byte = &H74
    Private Const PWR_MGMT_1 As Byte = &H6B '   default &H40 (sleep)
    Private Const PWR_MGMT_2 As Byte = &H6C
    Private Const SIG_PATH_RESET As Byte = &H68 '   bit0 - temp, bit1 - accel, bit2 - gyro

    Private Const GYRO_XOUT_H As Byte = &H43
    Private Const GYRO_XOUT_L As Byte = &H44
    Private Const GYRO_YOUT_H As Byte = &H45
    Private Const GYRO_YOUT_L As Byte = &H46
    Private Const GYRO_ZOUT_H As Byte = &H47
    Private Const GYRO_ZOUT_L As Byte = &H48

    Private Const ACCEL_XOUT_H As Byte = &H3B
    Private Const ACCEL_XOUT_L As Byte = &H3C
    Private Const ACCEL_YOUT_H As Byte = &H3D
    Private Const ACCEL_YOUT_L As Byte = &H3E
    Private Const ACCEL_ZOUT_H As Byte = &H3F
    Private Const ACCEL_ZOUT_L As Byte = &H40

    Private Const TEMP_OUT_H As Byte = &H41
    Private Const TEMP_OUT_L As Byte = &H42




    '   NRF

    Private Const NRF24_CE_PIN As Integer = 22           '   GPIO22
    Private Const NRF24_CONFIG As Byte = &H0             '   NORDIC SEMI NRF24L01 REGISTERS
    Private Const NRF24_ENA_AA As Byte = &H1
    Private Const NRF24_EN_RXADDR As Byte = &H2
    Private Const NRF24_SETUP_AW As Byte = &H3
    Private Const NRF24_SETUP_RETR As Byte = &H4
    Private Const NRF24_RF_CH As Byte = &H5
    Private Const NRF24_RF_SETUP As Byte = &H6
    Private Const NRF24_STATUS As Byte = &H7
    Private Const NRF24_OBSERVE_TX As Byte = &H8
    Private Const NRF24_CD As Byte = &H9
    Private Const NRF24_TX_ADDR As Byte = &H10
    Private Const NRF24_RX_ADD_P0 As Byte = &HA
    Private Const NRF24_RX_ADD_P1 As Byte = &HB
    Private Const NRF24_RX_ADD_P2 As Byte = &HC
    Private Const NRF24_RX_ADD_P3 As Byte = &HD
    Private Const NRF24_RX_ADD_P4 As Byte = &HE
    Private Const NRF24_RX_ADD_P5 As Byte = &HF

    Private Const NRF24_FIFO_STATUS As Byte = &H17

    Private Const NRF24_RD_RX_PAYLOAD As Byte = &H61    'commands
    Private Const NRF24_WR_TX_PAYLOAD As Byte = &HA0
    Private Const NRF24_FLUSH_TX As Byte = &HE1
    Private Const NRF24_FLUSH_RX As Byte = &HE2
    Private Const SPI_CHIP_SELECT_LINE As Integer = 0    ' Line 0 maps To physical pin number 24 On the RPi2 Or RPi3 CE0 - GPIO8
    Private Const SPI_CONTROLLER_NAME As String = "SPI0"


    '   Browser

    Public MyBookmarks(10, 10) As String
    Private GoogleHome As String = "http://www.google.ca"
    Private BingHome As String = "https://www.bing.com/#"

    '   Compass

    Private Const COMPASS_I2C_ADDRESS As Byte = &H1E        'HMC5883L electronic compass i2c address
    Private Const COMPASS_CONFIG_REGISTER_A As Byte = &H0
    Private Const COMPASS_CONFIG_REGISTER_B As Byte = &H1
    Private Const COMPASS_MODE_ADDRESS As Byte = &H2        'compass mode address
    Private Const COMPASS_XVALUE_H_REGISTER_ADDRESS As Byte = &H3
    Private Const COMPASS_XVALUE_L_REGISTER_ADDRESS As Byte = &H4
    Private Const COMPASS_ZVALUE_H_REGISTER_ADDRESS As Byte = &H5
    Private Const COMPASS_ZVALUE_L_REGISTER_ADDRESS As Byte = &H6
    Private Const COMPASS_YVALUE_H_REGISTER_ADDRESS As Byte = &H7
    Private Const COMPASS_YVALUE_L_REGISTER_ADDRESS As Byte = &H8
    Private Const COMPASS_STATUS_REGISTER_ADDRESS As Byte = &H9
    Private Const COMPASS_IDENT_A_REGISTER_ADDRESS As Byte = &H10
    Private Const COMPASS_IDENT_B_REGISTER_ADDRESS As Byte = &H11
    Private Const COMPASS_IDENT_C_REGISTER_ADDRESS As Byte = &H12

    Private CompassValue As Integer = 0
    Private compassBuffer As Byte()
    Private compassXBuffer As Byte    'local copy of compass X buffer
    Private compassYBuffer As Byte
    Private compassZBuffer As Byte

    Public Enum CompassOperatingMode
        CONTINUOUS_OPERATING_MODE = &H0
        SINGLE_OPERATING_MODE = &H1
        IDLE_OPERATING_MODE = &H10
    End Enum



    '   IO setup
    Private Shared gpio As GpioController = Nothing         ' GPIO 
    Public Property I2cPortExpander As I2cDevice            ' PORT EXPANDER - &H20 - MCP23017
    Public Property I2cGyro As I2cDevice                    ' GY-521 MPU6050 &H68
    Public Property I2cCompass As I2cDevice                 ' GY-271 COMPASS
    Public Property I2cMicroChip As I2cDevice               ' support chip for LYNX serial GPS - still deciding! 
    Public Property I2cAltimeter As I2cDevice               ' &H60 - MPL3115-A2 &H60
    Public Property IoController As GpioController
    Public Property NrfCEPin As GpioPin
    Public Property Nrf As SpiDevice

    Private Const I2C_CONTROLLER_NAME As String = "I2C1"





    '   Speech Recognition and Synthesis and Audio
    Dim MyListener As New SpeechRecognizer
    Dim MySpeaker As New SpeechSynthesizer
    Dim MySpeechStream As SpeechSynthesisStream
    Public MyBackgroundAudioStream As Stream
    Private isPlaying As Boolean = False
    Private isListening As Boolean = False
    Private isTalking As Boolean = False
    Private voice As VoiceInformation
    Private Const SRGS_FILE As String = "Grammar/Main1grammar.xml"   ' Grammar File
    Private Const mcbeth As String = "SSML/McBeth.xml"
    Private Const Jabberwoky As String = "SSML/jabberwoky.xml"

    Private IntroText As String = "Dragonfly has restarted, standby."

    Private EvaUS As String = "Eva"     'us english female
    Private Zira As String = "Zira"     'us english female
    Private David As String = "David"   'us english male
    Private Mark As String = "Mark"     'us english male

    Private Eva As String = "Eva"           'ca english female
    Private Linda As String = "Linda"       'ca english female
    Private Richard As String = "Richard"   'ca english male

    Private Hazel As String = "Hazel"     'gb english female
    Private Sarah As String = "Sarah"     'gb english female
    Private Susan As String = "Susan"     'gb english female
    Private George As String = "George"   'gb english male

    Private Catherine As String = "Catherine"    'au english female
    Private James As String = "James"            'au english male
    Private Matilda As String = "Matilda"        'au english female

    Private Claude As String = "Claude"        'ca french male
    Private CarolineCA As String = "Caroline"  'ca french female
    Private NathalieCA As String = "Nathalie"  'ca french female

    Private Paul As String = "Paul"           'fr french male
    Private Hortense As String = "Hortense"   'fr french male    
    Private CarolineFR As String = "Caroline" 'fr french female
    Private NathalieFR As String = "Nathalie" 'fr french female

    Private Hedda As String = "Hedda"   'german female
    Private Katja As String = "Katja"   'german female
    Private Stefan As String = "Stefan" 'german male

    Private Laura As String = "Laura"   'spanish female
    Private Helena As String = "Helena" 'spanish female
    Private Pablo As String = "Pablo"   'spanish male

    Private Cosimo As String = "Cosimo" 'italian male
    Private Elsa As String = "Elsa"     'italian female

    Private Haruka As String = "Haruka" 'japanese male
    Private Ichiro As String = "Ichiro" 'japanese male
    Private Ayumi As String = "Ayumi"   'japanese female
    Private Sayaka As String = "Sayaka" 'japanese female

    Private BodyText As String = "This is a sample of speech available, if you would like to play around with this app, add a method to select a voice from the available choices"
    Private Body1Text As String = "Also I would like to be able to select audio destination from the available choices"
    Private HelloEnglish As String = "Hello"
    Private HelloGerman As String = "Hallo"
    Private HelloFrench As String = "Bonjour"
    Private HelloSpanish As String = "Hola"
    Private HelloItalian As String = "Ciao"
    Private HelloJapanese As String = "Kon'nichiwa"

    Private StartTextEnglish As String = "I am listening"
    Private StartTextFrench As String = "j'écoute"
    Private StartTextSpanish As String = "estoy escuchando"
    Private StartTextGerman As String = "ich höre zu"
    Private StartTextItalian As String = "sto ascoltando"
    Private StartTextJapanese As String = "Kiite iru"
    Private StopTextEnglish As String = "Goodbye"
    Private StopTextGerman As String = "Auf Wiedersehen"
    Private StopTextFrench As String = "Au Revoir"
    Private StopTextSpanish As String = "Adiós"
    Private StopTextItalian As String = "addio"
    Private StopTextJapanese As String = "Sayonara"
    Private IntroTextEnglish As String = "Welcome to the Pi  home screen"
    Private IntroTextFrench As String = "Bienvenue dans l'écran d'accueil de Pi"
    Private IntroTextGerman As String = "Willkommen auf dem Pi Startbildschirm"
    Private IntroTextSpanish As String = "Bienvenido a la pantalla de inicio de Pi"
    Private IntroTextItalian As String = "Benvenuti alla schermata iniziale Pi"
    Private IntroTextJapanese As String = "Pi homu gamen e yokoso"

    '   Filer Stuff  Media Stuff...
    Dim currentFolder As StorageFolder
    Dim PickerSelectedFile As StorageFile
    Dim SelectedSong As StorageFile
    Public queryOptions As QueryOptions

    Private mediaFileExtensions As String() = {
        ".qcp", ".wav", ".mp3", ".m4r", ".m4a", ".aac", ".dat",
        ".amr", ".wma", ".3g2", ".3gp", ".mp4", ".wm", ".gif",
        ".asf", ".3gpp", ".3gp2", ".mpa", ".adt", ".adts",
        ".pya", ".wm", ".m4v", ".wmv", ".asf", ".mov", ".jpeg",
        ".mp4", ".3g2", ".3gp", ".mp4v", ".avi", ".pyv", ".tiff",
        ".3gpp", ".3gp2", ".bmp", ".png", ".jpg", ".txt", ".json"}

    '   Network Stuff

    Private OOBENetworkPageDispatcher As CoreDispatcher
    Private Automatic As Boolean = True
    Private AdminPassword As String = String.Empty
    Private CurrentPassword As String = String.Empty
    Private UserNetPass As PasswordCredential = Nothing
    Public ReadOnly Property NetworkReport As WiFiNetworkReport
    Dim networks As IList(Of WiFiAvailableNetwork)
    Public ReadOnly Property Timestamp As DateTimeOffset
    Public Event NetworkProfileChanged(ByVal sender As NetworkProfileChangedEventHandler)
    Public Event OnLocationChanged(ByVal sender As OnLocationChangedEventHandler)
    Public Event CharacterReceived(ByVal sender As CharacterReceivedEventArgs)

    '   USB
    Public ReadOnly Property USBDevicesReport As IList(Of UsbDeviceDescriptor)
    Public Event OnDeviceAdded(sender As OnDeviceAddedEventHandler)
    Public Event OnDeviceRemoved(ByVal sender As OnDeviceRemovedEventHandler)
    Public Class ConnectedDevice
        Public Property Id As String
        Public Property Name As String
    End Class



    '   All Radios

    Public ReadOnly Property KindofRadio As RadioKind



    '   Create a few system defaults
    Public Property XMax As Integer = 800
    Public Property YMax As Integer = 480
    Public Property MyRotation As Integer = 0
    Public Property MyXPos As Integer
    Public Property MyYPos As Integer
    Public Property MyZPos As Integer



    '   System stuff
    Private systemPresenter As SystemProperties
    Public MyAlarmPoints As TimeSpan()

    Public Class MYSYSTEMDEVICES
        Property GYRO As Boolean
        Property COMPASS As Boolean
        Property GPS As Boolean
        Property FINGERPRINT As Boolean
        Property NRF24 As Boolean
        Property MOTION As Boolean
    End Class
    Public Class MYRADIO
        Property Power As Integer
        Property Channel As Integer
        Property Pipe As Integer
        Property Payload As SByte
    End Class

    '   Initialize all parms
    Public Enum DeviceTypes
        RPI2
        RPI3
        MBM
        DB410
        GenericBoard
        Unknown
    End Enum

    Public Enum Location
        Name
        Country
        Location_Geobaseid
    End Enum

    '   ******  Start Here  *********
    Private Sub MainPage_Ready() Handles Me.Loaded
        queryOptions = New QueryOptions(CommonFileQuery.OrderByName, mediaFileExtensions) With {
            .FolderDepth = FolderDepth.Shallow
        }
        InitDragonfly()
        ShowDragonfly()
        StartDragonfly()
        WatchDevices()

    End Sub


    '   init dragonfly
    Private Sub InitDragonfly()

        StartClockTimers()
        HideAllGrids()
        RestoreMainGrids()
        LoadBookmarks()
        MyBrowser.Navigate(New Uri(GoogleHome))
        MediaElement.Volume = 0.15
        MainVolumeSlider.Value = 0.15
        Talker(David, IntroText)
        VoiceSelector.Items.Clear()
        Dim list = From a In SpeechSynthesizer.AllVoices
        For Each voice In list
            VoiceSelector.Items.Add(voice.DisplayName)
        Next
        InitCompass()
        SetCompassOperatingMode(CompassOperatingMode.CONTINUOUS_OPERATING_MODE)
    End Sub
    '   show dragonfly
    Private Sub ShowDragonfly()
        HideAllGrids()
        RestoreMainGrids()
    End Sub
    '   Start DragonFly - Jump to user App and back!
    Private Sub StartDragonfly()
        'more stuff
    End Sub


    '   ALL USB Stuff




    Public Sub WatchDevices()


    End Sub

    Private Sub DevicesEnumCompleted(ByVal sender As DeviceWatcher, ByVal args As DeviceWatcherEvent)

        ShowStatus("USB Devices Enumeration Completed")
    End Sub
    Private Sub DevicesAdded(ByVal sender As DeviceWatcher, ByVal args As DeviceInformation)
        ShowStatus("USB Devices Added: " & args.Id)
        Dim device = New ConnectedDevice() With {.Id = args.Id, .Name = args.Name}

        If Not UsbConnectedDevicesList.Items.Contains(device.Name) Then
            UsbConnectedDevicesList.Items.Add(device.Name)
        End If


    End Sub


    Private Sub ScanUSB()
        UsbConnectedDevicesList.Items.Clear()
        UsbConnectedDevicesList.Items.Add("nothing is plugged in")
        Try

        Catch ex As Exception

        End Try

    End Sub


    '   NRF24
    Private Sub InitNrf()
        Init_Nrf_CE()
        InitNRF24()
    End Sub
    '   Look for devices



    ' Initialize the SPI bus with the nrf24 attached if found.
    Private Async Sub InitNRF24()
        Try
            Dim settings = New SpiConnectionSettings(SPI_CHIP_SELECT_LINE)
            ' Create SPI initialization settings                               
            settings.ClockFrequency = 5000000
            ' Datasheet specifies maximum SPI clock frequency of 10MHz         
            settings.Mode = SpiMode.Mode3 ' was mode0
            Dim spiAqs As String = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME)
            ' Find the selector string for the SPI bus controller          
            Dim devicesInfo = Await DeviceInformation.FindAllAsync(spiAqs)
            Nrf = Await SpiDevice.FromIdAsync(devicesInfo(0).Id, settings)
            ' Add chip specific initialization here (NRF2401L)
            ' Read the status register to see if there is a radio attached.
        Catch ex As Exception
            Throw New Exception("NRF24 initialization failed", ex)
        End Try

    End Sub

    ' Initialize the GPIO pin for the NRF24 CE signal
    Private Sub Init_Nrf_CE()
        Try
            IoController = GpioController.GetDefault()
            ' Get the default GPIO controller on the system 
            ' Initialize a pin as output for the NRF24 CE line to the transciever  
            NrfCEPin = IoController.OpenPin(NRF24_CE_PIN)
            NrfCEPin.Write(GpioPinValue.High)
            NrfCEPin.SetDriveMode(GpioPinDriveMode.Output)
        Catch err As Exception
            Throw New Exception("GPIO initialization failed", err)
        End Try
    End Sub





    '   Browser Code
    Private Sub LoadBookmarks()

        MyBookmarks(0, 0) = "My MSN"
        MyBookmarks(0, 1) = "http://www.msn.com/en-ca?checklang=1"
        MyBookmarks(1, 0) = "Bing"
        MyBookmarks(1, 1) = "https://www.bing.com/#"
        MyBookmarks(2, 0) = "Raspberry Pi ORG"
        MyBookmarks(2, 1) = "https://www.raspberrypi.org/"
        MyBookmarks(3, 0) = "Windows Dev Center"
        MyBookmarks(3, 1) = "https://developer.microsoft.com/en-us/windows/iot/docs/releasenotesinsiderpreview"
        MyBookmarks(4, 0) = "CTV"
        MyBookmarks(4, 1) = "http://www.ctv.ca/"
        MyBookmarks(5, 0) = "Discovery Channel"
        MyBookmarks(5, 1) = "http://www.discovery.ca/"
        MyBookmarks(6, 0) = "GlobalTV"
        MyBookmarks(6, 1) = "http://www.globaltv.com/"
        BookmarksComboBox.Items.Add(MyBookmarks(0, 0))
        BookmarksComboBox.Items.Add(MyBookmarks(1, 0))
        BookmarksComboBox.Items.Add(MyBookmarks(2, 0))
        BookmarksComboBox.Items.Add(MyBookmarks(3, 0))
        BookmarksComboBox.Items.Add(MyBookmarks(4, 0))
        BookmarksComboBox.Items.Add(MyBookmarks(5, 0))
        BookmarksComboBox.Items.Add(MyBookmarks(6, 0))
    End Sub
    Private Sub ComboBox_SelectionChanged(sender As Object, args As SelectionChangedEventArgs) Handles BookmarksComboBox.SelectionChanged
        webAddressTxt.Text = MyBookmarks(BookmarksComboBox.SelectedIndex, 1)
        MyBrowser.Navigate(New Uri(MyBookmarks(BookmarksComboBox.SelectedIndex, 1)))
    End Sub
    '   back
    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs) Handles BrowserBackBtn.Click
        If MyBrowser.CanGoBack Then
            MyBrowser.GoBack()
        End If
    End Sub
    '   forward
    Private Sub ForwardBtn_Click(sender As Object, e As RoutedEventArgs) Handles BrowserForwardBtn.Click
        If MyBrowser.CanGoForward Then
            MyBrowser.GoForward()
        End If
    End Sub



    '   Compass
    Public Async Sub InitCompass()
        Try
            Dim i2cSettings = New I2cConnectionSettings(COMPASS_I2C_ADDRESS)
            i2cSettings.BusSpeed = I2cBusSpeed.FastMode
            Dim deviceSelector As String = I2cDevice.GetDeviceSelector(I2C_CONTROLLER_NAME)
            Dim i2cDeviceControllers = Await DeviceInformation.FindAllAsync(deviceSelector)
            I2cCompass = Await I2cDevice.FromIdAsync(i2cDeviceControllers(0).Id, i2cSettings)
            I2cCompass.WriteRead(New Byte() {COMPASS_MODE_ADDRESS}, compassBuffer)
        Catch err As Exception
            Debug.WriteLine("Exception: {0}", err.Message)
            Return
        End Try

        Try
            ' initialize local copies of the Compass registers
            compassBuffer = New Byte(0) {}
            I2cCompass.WriteRead(New Byte() {COMPASS_XVALUE_L_REGISTER_ADDRESS}, compassBuffer)
            compassXBuffer = compassBuffer(0)
            I2cCompass.WriteRead(New Byte() {COMPASS_YVALUE_L_REGISTER_ADDRESS}, compassBuffer)
            compassYBuffer = compassBuffer(0)
            I2cCompass.WriteRead(New Byte() {COMPASS_ZVALUE_L_REGISTER_ADDRESS}, compassBuffer)
            compassZBuffer = compassBuffer(0)

        Catch err As Exception
            Debug.WriteLine("Exception: {0}", err.Message)
            Return
        End Try
    End Sub

    Public Function GetDeviceId() As Byte()
        Dim identificationBufferA = New Byte(0) {}
        Dim identificationBufferB = New Byte(0) {}
        Dim identificationBufferC = New Byte(0) {}

        I2cCompass.WriteRead({COMPASS_IDENT_A_REGISTER_ADDRESS}, identificationBufferA)
        I2cCompass.WriteRead({COMPASS_IDENT_B_REGISTER_ADDRESS}, identificationBufferB)
        I2cCompass.WriteRead({COMPASS_IDENT_C_REGISTER_ADDRESS}, identificationBufferC)
        Return New Byte(2) {identificationBufferA(0), identificationBufferB(0), identificationBufferC(0)}
    End Function

    Public Sub SetCompassOperatingMode(operatingMode As CompassOperatingMode)
        Try
            ' convention is to specify the register first, and then the value to write to it
            Dim writeBuffer = New Byte(1) {COMPASS_MODE_ADDRESS, (operatingMode)}
            I2cCompass.Write(writeBuffer)
        Catch err As Exception
            Debug.WriteLine("Exception: {0}", err.Message)
        End Try

    End Sub

    Private Sub UpdateCompass()
        'textBlockX.Text = compassXBuffer
        'textBlockY.Text = compassYBuffer
        'textBlockZ.Text = compassZBuffer
    End Sub

    Private Sub ReadCompass()
        Try
            compassBuffer = New Byte(0) {}
            I2cCompass.WriteRead(New Byte() {COMPASS_XVALUE_L_REGISTER_ADDRESS}, compassBuffer)
            compassXBuffer = compassBuffer(0)
            I2cCompass.WriteRead(New Byte() {COMPASS_YVALUE_L_REGISTER_ADDRESS}, compassBuffer)
            compassYBuffer = compassBuffer(0)
            I2cCompass.WriteRead(New Byte() {COMPASS_ZVALUE_L_REGISTER_ADDRESS}, compassBuffer)
            compassZBuffer = compassBuffer(0)
        Catch err As Exception
            Debug.WriteLine("Exception: {0}", err.Message)
            Return
        End Try
    End Sub
    '   TODO MPU6050 stuff here...




    '   power management button brings up the power options menu
    Private Sub PowerBtn_Click(sender As Object, e As RoutedEventArgs) Handles PowerBtn.Click
        Select Case PowerOptionsGrid.Visibility
            Case Visibility.Collapsed
                PowerOptionsGrid.Visibility = Visibility.Visible
                Exit Select
            Case Visibility.Visible
                PowerOptionsGrid.Visibility = Visibility.Collapsed
        End Select
    End Sub

    Private Sub ShutDownBtn_Click(sender As Object, e As RoutedEventArgs) Handles ShutDownBtn.Click
        Talker(Zira, "Shutting down")
        ShutDown()
    End Sub

    Private Sub ShutDown()
        ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, TimeSpan.FromSeconds(1))
    End Sub

    Private Sub ReBootBtn_Click(sender As Object, e As RoutedEventArgs) Handles ReBootBtn.Click
        Talker(Zira, "rebooting")
        ReBoot()
    End Sub

    Private Sub ReBoot()
        ShutdownManager.BeginShutdown(ShutdownKind.Restart, TimeSpan.FromSeconds(1))
    End Sub

    '   Settings Button
    Private Sub SettingsAccessBtn_Click(sender As Object, e As RoutedEventArgs) Handles SettingsAccessBtn.Click
        ShowSettings()
    End Sub
    '   settings grid
    Private Sub ShowSettings()
        HideAllGrids()
        SettingsGrid.Visibility = Visibility.Visible
        GetWallpaper()
    End Sub

    '   from the main page
    Private Sub SystemInfoDisplayBtn_Click(sender As Object, e As RoutedEventArgs) Handles SystemInfoDisplayBtn.Click
        Select Case SystemInfoGrid.Visibility
            Case Visibility.Collapsed
                HideAllGrids()
                ShowSystemInfo()
                ShowNetworkStatus()
                CheckForWifiAdapters()
                ScanUSB()
                Exit Select
            Case Visibility.Visible
                RestoreMainGrids()
                SystemInfoGrid.Visibility = Visibility.Collapsed
        End Select
    End Sub
    '   tools 1 menu toggle
    Private Sub ToolsDisplayBtn_Click(sender As Object, e As RoutedEventArgs) Handles ToolsDisplayBtn.Click
        Select Case ToolsMenuGrid.Visibility
            Case Visibility.Collapsed
                HideAllGrids()
                ToolsMenuGrid.Visibility = Visibility.Visible
                Exit Select
            Case Visibility.Visible
                ToolsMenuGrid.Visibility = Visibility.Collapsed
                RestoreMainGrids()
        End Select
    End Sub

    '   Show SYSTEM info
    Private Sub ShowSystemInfo()
        GetMemory()
        SystemInfoGrid.Visibility = Visibility.Visible
        ' get OS info
        Dim ai As AnalyticsVersionInfo = AnalyticsInfo.VersionInfo
        SystemFamilyTxt.Text = ai.DeviceFamily
        Dim sv As String = AnalyticsInfo.VersionInfo.DeviceFamilyVersion
        Dim v As ULong = ULong.Parse(sv)
        Dim v1 As ULong = (v And &HFFFF000000000000UL) >> 48
        Dim v2 As ULong = (v And &HFFFF00000000L) >> 32
        Dim v3 As ULong = (v And &HFFFF0000L) >> 16
        Dim v4 As ULong = (v And &HFFFFL)
        SystemVersionTxt.Text = "OS Version - " + v1.ToString + "." + v2.ToString + "." + v3.ToString + "." + v4.ToString

        ' get the device manufacturer and model name
        Dim eas As New EasClientDeviceInformation()
        DeviceManufacturerTxt.Text = "Mfgr: " + eas.SystemManufacturer
        DeviceModelTxt.Text = eas.SystemProductName

        ' get CPU type
        Dim curpack As Package = Package.Current
        SystemArchitectureTxt.Text = "CPU - " + curpack.Id.Architecture.ToString()

        '   about me
        ApplicationNameTxt.Text = curpack.DisplayName
        Dim pv As PackageVersion = curpack.Id.Version
        PackageVersionTxt.Text = "Ver. " + pv.Major.ToString + "." + pv.Minor.ToString + "." + pv.Build.ToString + " Revision :" + pv.Revision.ToString
    End Sub

    '   show network stuff
    Private Sub ShowNetworkStatus()

        HostNameTxt.Text = "Hostname: " + Dns.GetHostName

        For Each localHostName As HostName In NetworkInformation.GetHostNames()

            If localHostName.IPInformation IsNot Nothing Then
                If localHostName.Type = HostNameType.Ipv6 Then
                    IPv6AddressTxt.Text = localHostName.ToString()
                    Exit For
                End If
            End If
        Next
        For Each localHostName As HostName In NetworkInformation.GetHostNames()
            If localHostName.IPInformation IsNot Nothing Then
                If localHostName.Type = HostNameType.Ipv4 Then
                    IPv4AddressTxt.Text = localHostName.ToString()
                    Exit For
                End If
            End If
        Next

        Dim network = NetworkInformation.GetInternetConnectionProfile
        Dim level = network.GetNetworkConnectivityLevel
        Select Case level
            Case NetworkConnectivityLevel.InternetAccess
                NetworkAccessInfoTxt.Text = "Local and Internet access."
                Exit Select
            Case NetworkConnectivityLevel.ConstrainedInternetAccess
                NetworkAccessInfoTxt.Text = "Limited Internet access."
                Exit Select
            Case NetworkConnectivityLevel.LocalAccess
                NetworkAccessInfoTxt.Text = "Local access only."
                Exit Select
            Case NetworkConnectivityLevel.None
                NetworkAccessInfoTxt.Text = "No access!"
                NetworkConnectTypeTxt.Text = "none"
                Exit Select
            Case Else
                Exit Select
        End Select

        If (network.IsWlanConnectionProfile Or network.IsWwanConnectionProfile) Then
            NetworkConnectTypeTxt.Text = "none"
        Else
            NetworkConnectTypeTxt.Text = "Ethernet"
        End If

    End Sub

    '   Show status....
    Private Sub ShowStatus(stat As String)
        StatusGrid.Visibility = Visibility.Visible
        GeneralStatusTxt.Text = stat
    End Sub

    '   GRIDS STUFF - RESTORE MAIN GRIDS
    Private Sub RestoreMainGrids()
        HideAllGrids()
        ClockGrid.Visibility = Visibility.Collapsed
        DateGrid.Visibility = Visibility.Collapsed
        PowerBtn.Visibility = Visibility.Visible
    End Sub

    '   HIDE ALL GRIDS...
    Private Sub HideAllGrids()
        PictureGrid.Visibility = Visibility.Collapsed
        ClockGrid.Visibility = Visibility.Collapsed
        DateGrid.Visibility = Visibility.Collapsed
        CalendarGrid.Visibility = Visibility.Collapsed
        PowerOptionsGrid.Visibility = Visibility.Collapsed
        StatusGrid.Visibility = Visibility.Collapsed
        SettingsGrid.Visibility = Visibility.Collapsed
        WifiConnectGrid.Visibility = Visibility.Collapsed
        BluetoothActivityGrid.Visibility = Visibility.Collapsed
        AudioManagerGrid.Visibility = Visibility.Collapsed
        SystemInfoGrid.Visibility = Visibility.Collapsed
        ToolsMenuGrid.Visibility = Visibility.Collapsed
        FileManagerGrid.Visibility = Visibility.Collapsed
        LocationOrientationGrid.Visibility = Visibility.Collapsed
        BrowserGrid.Visibility = Visibility.Collapsed
        AutoGrid.Visibility = Visibility.Collapsed
        PowerBtn.Visibility = Visibility.Collapsed
        AdminGrid.Visibility = Visibility.Collapsed
        InstalledAppsGrid.Visibility = Visibility.Collapsed
        IATitleGrid.Visibility = Visibility.Collapsed
        WeatherGrid.Visibility = Visibility.Collapsed
        TimePickerGrid.Visibility = Visibility.Collapsed
        ToolsTwoGrid.Visibility = Visibility.Collapsed
        ConfigureGrid.Visibility = Visibility.Collapsed
        GuestMgmtGrid.Visibility = Visibility.Collapsed
    End Sub

    '   SPIN MY DRAGONFLY
    Private Sub UpdateDragonfly()
        Try
            MyRotation = MyRotation + 3
            If MyRotation > 359 Then MyRotation = 0
            Dim myrot As New RotateTransform With {.Angle = MyRotation}
            targetDragonflyImg.RenderTransform = myrot
        Catch ex As Exception

        End Try
    End Sub

    '   show memory
    Private Sub GetMemory()
        Dim memusage As Integer = CInt((MemoryManager.AppMemoryUsage / MemoryManager.AppMemoryUsageLimit) * 100)
        MemoryUsageInd.Value = memusage
        MemoryUsageTxt.Text = "Usage Level: " + MemoryManager.AppMemoryUsageLevel.ToString + " - " + memusage.ToString + " % used."
        Dim used As Integer = CInt(MemoryManager.AppMemoryUsage.ToString) / 1048576
        MemoryUsedTxt.Text = used.ToString + " MBytes"
    End Sub
    '   network status changed
    Private Sub NetworkProfile_Changed(sender As NetworkProfileChangedEventHandler)
        ShowNetworkStatus()
    End Sub
    '   character received
    Private Sub Character_Received(sender As CharacterReceivedEventArgs)

    End Sub

    Private Sub VoiceSelector_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles VoiceSelector.SelectionChanged

    End Sub

    '   ALL WIFI HERE
    Private Sub WifiConnectGridOKBtn_Click(sender As Object, e As RoutedEventArgs) Handles WifiConnectGridOKBtn.Click
        RestoreMainGrids()
    End Sub
    '   wifi access connect grid
    Private Sub WifiAccessBtn_Click(ByVal sender As Object, e As RoutedEventArgs) Handles WifiAccessBtn.Click
        ShowAvailableWifiNetworks()
    End Sub
    '   connect to a network
    Private Sub ConnectToNetworkBtn_Click(sender As Object, e As RoutedEventArgs) Handles ConnectToNetworkBtn.Click

    End Sub

    '   check for adapters
    Private Async Sub CheckForWifiAdapters()
        Dim result = Await DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector())
        Dim access = Await WiFiAdapter.RequestAccessAsync()
        If result.Count >= 1 Then
            If access <> WiFiAccessStatus.Allowed Then
                ShowStatus("WiFi Adapter! - No Access")
            Else
                ShowStatus("WiFi Adapter! - Wifi Access")
            End If
        Else
            ShowStatus("No WiFi Adapter!")
        End If
    End Sub

    '   Show available networks to connect to
    Private Async Sub ShowAvailableWifiNetworks()
        HideAllGrids()
        WifiConnectGrid.Visibility = Visibility.Visible
        CheckForWifiAdapters()
        Try
            AccessPoints.Items.Clear()
            Dim result = Await DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector())
            Dim firstAdapter = Await WiFiAdapter.FromIdAsync(result(0).Id)
            Await firstAdapter.ScanAsync()
            Dim AvailWiFiNetworks = firstAdapter.NetworkReport.AvailableNetworks

            If AvailWiFiNetworks.Count > 1 Then
                NetworkWifiInfoTxt.Text = "Networks are available"
                For Each element In AvailWiFiNetworks
                    Dim AvailNetwork As String = ""
                    AvailNetwork = "Name: " + element.Ssid.ToString

                    If NetworkAuthenticationType.Open80211 Then
                        AvailNetwork = AvailNetwork + " :OPEN"
                    ElseIf NetworkAuthenticationType.SharedKey80211 Then
                        AvailNetwork = AvailNetwork + " :SECURED"
                    ElseIf NetworkAuthenticationType.None Then
                        AvailNetwork = AvailNetwork + " none"
                    ElseIf NetworkAuthenticationType.Wpa Then
                        AvailNetwork = AvailNetwork + " WPA"
                    ElseIf NetworkAuthenticationType.WpaPsk Then
                        AvailNetwork = AvailNetwork + " WPA-PSK"
                    ElseIf NetworkAuthenticationType.WpaNone Then
                        AvailNetwork = AvailNetwork + " WPA-none"
                    End If

                    If WiFiNetworkKind.Adhoc Then
                        AvailNetwork = AvailNetwork + " -P2P"
                    ElseIf WiFiNetworkKind.Infrastructure Then
                        AvailNetwork = AvailNetwork + " -AP"
                    ElseIf WiFiNetworkKind.Any Then
                        AvailNetwork = AvailNetwork + " -ANY"
                    End If
                    AccessPoints.Items.Add(AvailNetwork)
                Next

            ElseIf AvailWiFiNetworks.Count < 1 Then
                NetworkWifiInfoTxt.Text = "No networks available"
            End If
        Catch ex As Exception

        End Try
        CheckForWifiAdapters()
    End Sub

    '   ******************************************************************************
    '   GEO LOCATION
    '   update current location
    Private Sub UpdateLocation(sender As StatusChangedEventArgs)
        GetLocation()
    End Sub

    Private Async Sub GetLocation()
        Try
            'HideAllGrids()
            'MapGrid.Visibility = Visibility.Visible
            Dim geolocator As New Geolocator()
            Dim position As Geoposition = Await geolocator.GetGeopositionAsync
            Dim venue = position.VenueData
            Dim center As Geopoint = Nothing

            ' myMapCtrl.ZoomLevel = 10

            ' Dim map = Await myMapCtrl.TrySetViewAsync(center)
            Dim lat = position.Coordinate.Point.Position.Latitude
            Dim lon = position.Coordinate.Point.Position.Longitude
            Dim alt = position.Coordinate.Point.Position.Altitude
            Dim dir = position.Coordinate.Heading
            Dim spd = position.Coordinate.Speed
            '  LatitudeTxt.Text = lat.ToString
            '  LongitudeTxt.Text = lon.ToString
        Catch ex As Exception

        End Try
    End Sub

    '   new grid management module
    Private Sub GridsStandby()

    End Sub
    '   init clocks and timers
    Private Sub StartClockTimers()
        'Create Timer
        Dim Clock As DispatcherTimer = New DispatcherTimer()
        AddHandler Clock.Tick, AddressOf OneSecTimer_Tick
        Clock.Interval = TimeSpan.FromSeconds(1)
        Clock.Start()
        'Start the 100 millisec count this handles the compass
        Dim milli_tmr As DispatcherTimer = New DispatcherTimer()
        AddHandler milli_tmr.Tick, AddressOf Quick_Tick
        milli_tmr.Interval = TimeSpan.FromMilliseconds(100)
        milli_tmr.Start()
        'Start a 5 millisec count
        Dim five_milli_tmr As DispatcherTimer = New DispatcherTimer()
        AddHandler five_milli_tmr.Tick, AddressOf FiveMilliSecTimer_Tick
        five_milli_tmr.Interval = TimeSpan.FromMilliseconds(2)
        five_milli_tmr.Start()
    End Sub
    '   timers
    Private Sub FiveMilliSecTimer_Tick(sender As Object, e As Object)
        UpdateDragonfly()
    End Sub
    '   timers
    Private Sub Quick_Tick(sender As Object, e As Object)
        If LocationOrientationGrid.Visibility = Visibility.Visible Then
            ReadCompass()
            CompassValue = compassXBuffer
            CompassImage.RenderTransformOrigin = New Point(0.5, 0.5)
            Dim myRotateTransform As New RotateTransform()
            myRotateTransform.Angle = 359 - CompassValue
            CompassImage.RenderTransform = myRotateTransform
        End If
    End Sub
    '   timers
    Private Sub OneSecTimer_Tick(ByVal sender As DispatcherTimer, args As Object)
        'currentTimeHourTxt.Text = Date.Now.ToString("hh:mm:ss tt zzz")
        currentTimeHourTxt.Text = Date.Now.ToString("hh")
        currentTimeMinutesTxt.Text = Date.Now.ToString("mm")
        currentTimeSecondsTxt.Text = Date.Now.ToString("ss")
        currentTimeAmPmTxt.Text = Date.Now.ToString("tt")
        'currentDateTxt.Text = Date.Now.ToString("dddd, MMMM, dd, yyyy")
        currentDateDayTxt.Text = Date.Now.ToString("dddd")
        currentDateMonthTxt.Text = Date.Now.ToString("MMMM")
        currentDateTxt.Text = Date.Now.ToString("dd")
        currentDateYearTxt.Text = Date.Now.ToString("yyyy")
    End Sub

    '   *****************************************************************
    '   Load the available backdrop files. 
    Private Async Sub GetWallpaper()
        Try
            WallpaperSelector.Items.Clear()
            Dim queryOption As New QueryOptions(CommonFileQuery.OrderByTitle, New String() {".jpg", ".png", ".bmp"}) With {
                .FolderDepth = FolderDepth.Deep
            }
            Dim folders As New Queue(Of IStorageFolder)()
            Dim files = Await KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(queryOption).GetFilesAsync()
            ' do something with the files
            For Each File In files
                WallpaperSelector.Items.Add(File.Name)
            Next
        Catch ex As Exception
            ShowStatus("no files")
        End Try
    End Sub
    '   Retreive the selected file
    Private Async Sub Wallpaper_SelectionChanged(ByVal sender As Object, e As SelectionChangedEventArgs) Handles WallpaperSelector.SelectionChanged
        Try
            WallpaperSw.IsOn = True
            Dim tempBit As New BitmapImage()
            Dim LoadedStorageFile As StorageFile = Await KnownFolders.PicturesLibrary.GetFileAsync(WallpaperSelector.SelectedItem)
            Using imagefilestream As IRandomAccessStream = Await LoadedStorageFile.OpenAsync(FileAccessMode.Read)
                Await tempBit.SetSourceAsync(imagefilestream)
            End Using
            ' point to the bitmap
            WallpaperImg.Source = tempBit
        Catch ex As Exception

        End Try
    End Sub
    '   wallpaper on/off
    Private Sub WallpaperSw_Toggled(sender As Object, e As RoutedEventArgs) Handles WallpaperSw.Toggled
        If WallpaperSw.IsOn = True Then
            WallpaperImg.Visibility = Visibility.Visible
            mainDragonflyImg.Visibility = Visibility.Collapsed
        ElseIf WallpaperSw.IsOn = False Then
            WallpaperImg.Visibility = Visibility.Collapsed
            mainDragonflyImg.Visibility = Visibility.Visible
        End If
    End Sub
    '   turns off settings grid
    Private Sub SettingsGridOKBtn_Click(sender As Object, e As RoutedEventArgs) Handles SettingsGridOKBtn.Click
        HideAllGrids()
        RestoreMainGrids()
    End Sub

    '   *****************************************************
    '   All Bluetooth stuff here

    Private devicePicker As DevicePicker = Nothing

    Public Event BTRefreshCompleted As EventHandler
    Public Event BTPairDisconnect As EventHandler
    Public Event BTPairConnected As EventHandler

    Public Async Function UnpairAsync(device As DeviceInformation) As Task(Of DeviceUnpairingResult)
        Return Await device.Pairing.UnpairAsync()
    End Function

    Public Async Function PairAsync(device As DeviceInformation) As Task(Of DevicePairingResult)
        Return Await device.Pairing.PairAsync(DevicePairingProtectionLevel.None)

    End Function

    '   bluetooth manager selected
    Private Sub BluetoothManagerBtn_Click(sender As Object, e As RoutedEventArgs) Handles BluetoothManagerBtn.Click
        HideAllGrids()
        BluetoothActivityGrid.Visibility = Visibility.Visible
        UpdateBluetoothDevices()
    End Sub
    '   close bluetooth manager menu
    Private Sub CloseBTMBtn_Click(sender As Object, e As RoutedEventArgs) Handles CloseBTMBtn.Click
        BluetoothActivityGrid.Visibility = Visibility.Collapsed
        RestoreMainGrids()
    End Sub

    '   enable/disable bluetooth
    Private Sub BluetoothEnableSw_Toggled(sender As Object, e As RoutedEventArgs) Handles BluetoothEnableSw.Toggled

        If BluetoothEnableSw.IsOn Then
            'bt on
            Talker(Zira, "Bluetooth has been enabled")
        ElseIf BluetoothEnableSw.IsOn <> True Then
            'bt off
            Talker(Zira, "Bluetooth has been turned off")
        End If


    End Sub
    '   shutdown bt
    Private Async Sub BluetoothShutdown()
        Try
            Dim accessLevel = Await Radio.RequestAccessAsync()
            If accessLevel > 1 Then
                Dim radios = Await Radio.GetRadiosAsync()
                Dim s = Radio.GetDeviceSelector(radios(0).State)

            End If

        Catch ex As Exception

        End Try

    End Sub
    '   bluetooth power up
    Private Sub BlueToothStart()
        Try

        Catch ex As Exception

        End Try
    End Sub
    '   Bluetooth Scan button
    Private Sub BluetoothScanBtn_Click(sender As Object, e As RoutedEventArgs) Handles BtScanBtn.Click

    End Sub
    '   pair BT
    Private Sub BluetoothPairBtn_Click(sender As Object, e As RoutedEventArgs) Handles BluetoothPairBtn.Click
        BluetoothPairBtn.IsEnabled = False
        '   pair away
        Dim item As New ObservableCollection(Of DeviceInformation)
        If item.Count > 1 Then

        End If


        '   once the devices are paired, enable the UN-PAIR button
        BluetoothUnPairBtn.IsEnabled = True

    End Sub
    '   unpair BT
    Private Sub BluetoothUnPairBtn_Click(sender As Object, e As RoutedEventArgs) Handles BluetoothUnPairBtn.Click
        BluetoothPairBtn.IsEnabled = True
        '   UnpairAsync("")
    End Sub
    '   bluetooth update
    Private Async Sub UpdateBluetoothDevices()
        Try

            PairedDevicesList.Items.Clear()
            BluetoothDeviceList.Items.Clear()

            Dim DIP = New ObservableCollection(Of DeviceInformationPairing)
            For Each Item In DIP
                PairedDevicesList.Items.Add(Item.ToString)
            Next

            Dim result = Await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelector())
            Dim bt As DeviceInformationCollection = Await DeviceInformation.FindAllAsync
            For Each Item In result
                BluetoothDeviceList.Items.Add(Item.Name)
            Next
        Catch ex As Exception

        End Try
    End Sub


    '   ************************************************************************
    '   MUSIC MANAGER -  from the tools menu
    Private Sub AudioManagerBtn_Click(sender As Object, e As RoutedEventArgs) Handles AudioManagerBtn.Click
        HideAllGrids()
        AudioManagerGrid.Visibility = Visibility.Visible
        ShowAllMusic()
    End Sub
    '   Music populate
    Private Async Sub ShowAllMusic()
        Await MusicPopulate()
    End Sub
    '   populate the music listbox
    Private Async Function MusicPopulate() As Task
        SelectedSong = Nothing
        If currentFolder Is Nothing Then
            MusicFilesList.Items.Clear()
            MusicFilesList.Items.Add(">Music")
            MusicFilesList.Items.Add(">RemovableStorage")
        Else
            MusicFilesList.Items.Clear()
            MusicFilesList.Items.Add(">..")
            Dim folders = Await currentFolder.GetFoldersAsync()
            For Each f In folders
                MusicFilesList.Items.Add(">" + f.Name)
            Next
            Dim query = currentFolder.CreateFileQueryWithOptions(queryOptions)
            Dim files = Await query.GetFilesAsync()
            For Each File In files
                MusicFilesList.Items.Add(File.Name)
            Next
        End If
    End Function
    Private Async Function BrowseToFolder(filename As String) As Task(Of Boolean)
        SelectedSong = Nothing
        If currentFolder Is Nothing Then
            Select Case filename
                Case ">Music"
                    currentFolder = KnownFolders.MusicLibrary
                    Exit Select
                Case ">RemovableStorage"
                    currentFolder = KnownFolders.RemovableDevices
                    Exit Select
                Case Else
                    'case NetworkFolder:
                    '    // special case... NYI
                    '    return false;
                    Throw New Exception("unexpected")
            End Select
            SongTxt.Text = "> " + filename.Substring(1)
        Else
            If filename = ">.." Then
                Await MusicFolderUp()
            ElseIf filename(0) = ">"c Then
                Dim foldername = filename.Substring(1)
                Dim folder = Await currentFolder.GetFolderAsync(foldername)
                currentFolder = folder
                SongTxt.Text += " > " + foldername
            Else
                SelectedSong = Await currentFolder.GetFileAsync(filename)
                Return True
            End If
        End If
        Await MusicPopulate()
        Return False
    End Function
    Private Async Function MusicFolderUp() As Task
        If currentFolder Is Nothing Then
            Return
        End If
        Try
            Dim folder = Await currentFolder.GetParentAsync()
            currentFolder = folder
            If currentFolder Is Nothing Then
                SongTxt.Text = ">"
            Else
                Dim breadcrumb = SongTxt.Text
                breadcrumb = breadcrumb.Substring(0, breadcrumb.LastIndexOf(">"c) - 1)
                SongTxt.Text = breadcrumb
            End If
        Catch generatedExceptionName As Exception
            currentFolder = Nothing
            SongTxt.Text = ">"
        End Try
    End Function
    Private Async Sub MusicFiles_KeyUp(ByVal sender As Object, ByVal args As KeyRoutedEventArgs) Handles MusicFilesList.KeyUp
        If MusicFilesList.SelectedItem IsNot Nothing AndAlso args.Key = Windows.System.VirtualKey.Enter Then
            If Await BrowseToFolder(MusicFilesList.SelectedItem.ToString()) Then
                SelectSong()
            Else
                MusicFilesList.Focus(FocusState.Keyboard)
            End If
        End If
    End Sub
    Private Async Sub MusicFiles_DoubleTapped(ByVal sender As Object, ByVal args As DoubleTappedRoutedEventArgs) Handles MusicFilesList.DoubleTapped
        If MusicFilesList.SelectedItem IsNot Nothing Then
            If Await BrowseToFolder(MusicFilesList.SelectedItem.ToString()) Then
                SelectSong()
            Else
                MusicFilesList.Focus(FocusState.Keyboard)
            End If
        End If
    End Sub
    Private Async Sub SelectSong()
        Try
            If SelectedSong IsNot Nothing Then
                SelectedMusicTxt.Text = SelectedSong.Path
                MyBackgroundAudioStream = Await SelectedSong.OpenAsync(FileAccessMode.Read)
                MediaElement.SetSource(stream:=MyBackgroundAudioStream, mimeType:=SelectedSong.ContentType)
            End If
        Catch ex As Exception
            ShowStatus(ex.Message.ToString)
        End Try
    End Sub

    '   music controls
    Private Sub PauseMusic()
        MediaElement.Pause()
    End Sub
    Private Sub ResumeMusic()
        MediaElement.SetSource(MyBackgroundAudioStream, PickerSelectedFile.ContentType)
    End Sub

    '   Audio volume
    Private Sub MainVolumeSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles MainVolumeSlider.ValueChanged
        MediaElement.Volume = MainVolumeSlider.Value
    End Sub

    '   close self (audio manager)
    Private Sub AudioManagerOKBtn_Click(sender As Object, e As RoutedEventArgs) Handles AudioManagerOKBtn.Click
        AudioManagerGrid.Visibility = Visibility.Collapsed
    End Sub


    '   *********************************************************
    '   Image file manager button
    Private Sub ImageViewerBtn_Click(sender As Object, e As RoutedEventArgs) Handles ImageViewerBtn.Click
        HideAllGrids()
        PictureGrid.Visibility = Visibility.Visible
        ImageFilePopulate()
    End Sub
    Private Sub CloseImageViewerBtn_Click(sender As Object, e As RoutedEventArgs) Handles CloseImageViewerBtn.Click
        PictureGrid.Visibility = Visibility.Collapsed
    End Sub
    '   Filer populate
    Private Async Sub ImageFilePopulate()
        Await Picker_Populate()
    End Sub
    '   Picker hide
    Private Sub PickerHide()
        PictureGrid.Visibility = Visibility.Collapsed
    End Sub
    '   Picker Populate
    Private Async Function Picker_Populate() As Task
        PickerSelectedFile = Nothing
        If currentFolder Is Nothing Then
            lstFiles.Items.Clear()
            lstFiles.Items.Add(">Pictures")
            lstFiles.Items.Add(">RemovableStorage")
        Else
            lstFiles.Items.Clear()
            lstFiles.Items.Add(">..")
            Dim folders = Await currentFolder.GetFoldersAsync()
            For Each f In folders
                lstFiles.Items.Add(">" + f.Name)
            Next
            Dim query = currentFolder.CreateFileQueryWithOptions(queryOptions)
            Dim files = Await query.GetFilesAsync()
            For Each File In files
                lstFiles.Items.Add(File.Name)
            Next
        End If
    End Function
    Private Async Function Picker_BrowseTo(filename As String) As Task(Of Boolean)
        PickerSelectedFile = Nothing
        If currentFolder Is Nothing Then
            Select Case filename
                Case ">Pictures"
                    currentFolder = KnownFolders.PicturesLibrary
                    Exit Select
                Case ">RemovableStorage"
                    currentFolder = KnownFolders.RemovableDevices
                    Exit Select
                Case Else
                    'case NetworkFolder:
                    '    // special case... NYI
                    '    return false;
                    Throw New Exception("unexpected")
            End Select
            lblBreadcrumb.Text = "> " + filename.Substring(1)
        Else
            If filename = ">.." Then
                Await Picker_FolderUp()
            ElseIf filename(0) = ">"c Then
                Dim foldername = filename.Substring(1)
                Dim folder = Await currentFolder.GetFolderAsync(foldername)
                currentFolder = folder
                lblBreadcrumb.Text += " > " + foldername
            Else
                PickerSelectedFile = Await currentFolder.GetFileAsync(filename)
                Return True
            End If
        End If
        Await Picker_Populate()
        Return False
    End Function
    Private Async Function Picker_FolderUp() As Task
        If currentFolder Is Nothing Then
            Return
        End If
        Try
            Dim folder = Await currentFolder.GetParentAsync()
            currentFolder = folder
            If currentFolder Is Nothing Then
                lblBreadcrumb.Text = ">"
            Else
                Dim breadcrumb = lblBreadcrumb.Text
                breadcrumb = breadcrumb.Substring(0, breadcrumb.LastIndexOf(">"c) - 1)
                lblBreadcrumb.Text = breadcrumb
            End If
        Catch generatedExceptionName As Exception
            currentFolder = Nothing
            lblBreadcrumb.Text = ">"
        End Try
    End Function

    Private Async Sub LstFiles_KeyUp(ByVal sender As Object, ByVal args As KeyRoutedEventArgs) Handles lstFiles.KeyUp
        If lstFiles.SelectedItem IsNot Nothing AndAlso args.Key = Windows.System.VirtualKey.Enter Then
            If Await Picker_BrowseTo(lstFiles.SelectedItem.ToString()) Then
                SelectFile()
            Else
                lstFiles.Focus(FocusState.Keyboard)
            End If
        End If
    End Sub
    Private Async Sub LstFiles_DoubleTapped(ByVal sender As Object, ByVal args As DoubleTappedRoutedEventArgs) Handles lstFiles.DoubleTapped
        If lstFiles.SelectedItem IsNot Nothing Then
            If Await Picker_BrowseTo(lstFiles.SelectedItem.ToString()) Then
                SelectFile()
            Else
                lstFiles.Focus(FocusState.Keyboard)
            End If
        End If
    End Sub
    Private Async Sub SelectFile()
        Dim tempBit As New BitmapImage()
        Try
            If PickerSelectedFile IsNot Nothing Then
                txtFileName.Text = PickerSelectedFile.Path
                Dim ImageFileStream = Await PickerSelectedFile.OpenAsync(FileAccessMode.Read)
                Await tempBit.SetSourceAsync(ImageFileStream)
                previewImage.Source = tempBit
            End If
        Catch ex As Exception
            ShowStatus(ex.Message.ToString)
        End Try
    End Sub


    '   *************************************************************************
    '   Speech stuff here...
    Private Sub SpeechPlusBtn_Click(sender As Object, e As RoutedEventArgs) Handles SpeechPlusBtn.Click
        HideAllGrids()
        SpeechPlusGrid.Visibility = Visibility.Visible
    End Sub
    Private Async Sub Start_Listening()
        Dim result As SpeechRecognitionResult = Await MyListener.RecognizeAsync()
    End Sub
    Private Async Sub Stop_Listening()
        Await MyListener.StopRecognitionAsync()
        Await MyListener.ContinuousRecognitionSession.StopAsync()
    End Sub
    Private Sub Set_Voice(name)
        Dim voice = SpeechSynthesizer.AllVoices.Single(Function(v) v.DisplayName.Contains(name))
        MySpeaker.Voice = voice
    End Sub
    Private Async Sub Talker(voice, message)
        If isPlaying Then

        End If
        Try
            If (Not isTalking) Then
                isTalking = True
                MySpeaker.Voice = SpeechSynthesizer.AllVoices.FirstOrDefault(Function(v) v.DisplayName.Contains(voice))
                MySpeechStream = Await MySpeaker.SynthesizeTextToStreamAsync(message)
                MediaElement.SetSource(MySpeechStream, MySpeechStream.ContentType)
                isTalking = False
            End If
        Catch ex As Exception
            ShowStatus(ex.ToString)
        End Try
    End Sub
    '   Close SpeechPlus Grid
    Private Sub CloseSpeechPlusBtn_Click(sender As Object, e As RoutedEventArgs) Handles CloseSpeechPlusBtn.Click
        SpeechPlusGrid.Visibility = Visibility.Collapsed
        RestoreMainGrids()
    End Sub
    '   listen on in Speech Plus grid
    Private Sub ListenEnableSw_Toggled(sender As Object, e As RoutedEventArgs) Handles ListenEnableSw.Toggled
        If ListenEnableSw.IsOn Then
            Stop_Listening()
        Else
            Start_Listening()
        End If
    End Sub

    Private Sub GeoBtn_Click(sender As Object, e As RoutedEventArgs) Handles GeoBtn.Click
        '   toggle the Geo grid
        Select Case LocationOrientationGrid.Visibility
            Case Visibility.Visible
                LocationOrientationGrid.Visibility = Visibility.Collapsed
                Exit Select
            Case Visibility.Collapsed
                HideAllGrids()
                LocationOrientationGrid.Visibility = Visibility.Visible
        End Select
    End Sub
    '   Browser grid *******************************************
    Private Sub BrowserBtn_Click(sender As Object, e As RoutedEventArgs) Handles BrowserBtn.Click

        Select Case BrowserGrid.Visibility
            Case Visibility.Visible
                BrowserGrid.Visibility = Visibility.Collapsed
                PowerBtn.Visibility = Visibility.Visible
                Exit Select
            Case Visibility.Collapsed
                PowerBtn.Visibility = Visibility.Collapsed
                BrowserGrid.Visibility = Visibility.Visible
        End Select

    End Sub
    '   Automation grid ***************
    Private Sub AutomationBtn_Click(sender As Object, ByVal args As RoutedEventArgs) Handles AutomationBtn.Click
        Select Case AutoGrid.Visibility
            Case Visibility.Visible
                AutoGrid.Visibility = Visibility.Collapsed
                RestoreMainGrids()
                Exit Select
            Case Visibility.Collapsed
                HideAllGrids()
                Talker(Zira, "The automation system is offline, no NRF24 found")
                AutoGrid.Visibility = Visibility.Visible
        End Select
    End Sub
    '   Close browser
    Private Sub BrowserCloseBtn_Click(sender As Object, e As RoutedEventArgs) Handles BrowserCloseBtn.Click
        HideAllGrids()
        RestoreMainGrids()
    End Sub
    '   Apps menu
    Private Sub AppsMenuBtn_ClickAsync(sender As Object, e As RoutedEventArgs) Handles AppsMenuBtn.Click
        Select Case IATitleGrid.Visibility
            Case Visibility.Visible
                RestoreMainGrids()
                InstalledAppsGrid.Visibility = Visibility.Collapsed
                IATitleGrid.Visibility = Visibility.Collapsed
                Exit Select
            Case Visibility.Collapsed
                HideAllGrids()
                InstalledAppsGrid.Visibility = Visibility.Visible
                IATitleGrid.Visibility = Visibility.Visible
                GetAllAvailableApps()
        End Select
    End Sub
    Private Async Sub GetAllAvailableApps()
        Dim allapps = Await AppServiceCatalog.FindAppServiceProvidersAsync("Cortana")
        Dim packageName = Nothing
        If allapps.Count = 1 Then
            packageName = allapps(0).PackageFamilyName
            InstalledAppsListView.Items.Add(packageName.ToString)
        End If

    End Sub
    '   Show the Admin Grid
    Private Sub AdminBtn_Click(sender As Object, e As RoutedEventArgs)
        Select Case AdminGrid.Visibility
            Case Visibility.Collapsed
                HideAllGrids()
                AdminGrid.Visibility = Visibility.Visible

                Exit Select
            Case Visibility.Visible
                RestoreMainGrids()
                AdminGrid.Visibility = Visibility.Collapsed
        End Select
    End Sub
    '   Show the Clock grid
    Private Sub ClockCalendarShowBtn_Click(sender As Object, e As RoutedEventArgs) Handles ClockCalendarShowBtn.Click
        Select Case ClockGrid.Visibility
            Case Visibility.Visible
                ClockGrid.Visibility = Visibility.Collapsed
                DateGrid.Visibility = Visibility.Collapsed
                Exit Select
            Case Visibility.Collapsed
                HideAllGrids()
                ClockGrid.Visibility = Visibility.Visible
                DateGrid.Visibility = Visibility.Visible
        End Select
    End Sub
    '   Time grid double tapped
    Private Sub ClockGrid_DoubleTapped(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles ClockGrid.DoubleTapped
        HideAllGrids()
        TimePickerGrid.Visibility = Visibility.Visible
    End Sub
    Private Sub CancelAlarmTimeBtn_Click(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles CancelAlarmTimeBtn.Click
        TimePickerGrid.Visibility = Visibility.Collapsed
        RestoreMainGrids()
        ShowStatus("Alarm cancelled!")
    End Sub
    Private Sub SetAlarmTimeBtn_Click(sender As Object, ByVal args As RoutedEventArgs) Handles SetAlarmTimeBtn.Click
        TimePickerGrid.Visibility = Visibility.Collapsed
        SetAlarmTime()
    End Sub
    Private Sub SetAlarmTime()
        '   set alarm

        RestoreMainGrids()
        ShowStatus("Alarm set!")
    End Sub
    '   Date grid double tapped
    Private Sub DateGrid_DoubleTapped(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles DateGrid.DoubleTapped
        HideAllGrids()
        ShowCalendarGrid()
    End Sub
    '   Show calendar
    Private Sub ShowCalendarGrid()
        CalendarGrid.Visibility = Visibility.Visible
    End Sub

    '   show weather  
    Private Sub ShowWeatherBtn_Click(sender As Object, ByVal args As RoutedEventArgs) Handles ShowWeatherBtn.Click
        ToolsMenuGrid.Visibility = Visibility.Collapsed
        WeatherGrid.Visibility = Visibility.Visible
    End Sub
    '   Scene Lights button
    Private Sub SceneLightsBtn_Click(sender As Object, ByVal args As RoutedEventArgs) Handles SceneLightsBtn.Click

    End Sub

    Private Sub AdminOKBtn_Click(sender As Object, ByVal args As RoutedEventArgs) Handles AdminOKBtn.Click
        RestoreMainGrids()
    End Sub


    Private Sub MoreToolsBtn_Click(sender As Object, e As RoutedEventArgs) Handles MoreToolsBtn.Click
        ToolsMenuGrid.Visibility = Visibility.Collapsed
        ToolsTwoGrid.Visibility = Visibility.Visible
    End Sub

    Private Sub ConfigureBtn_Click(sender As Object, e As RoutedEventArgs) Handles ConfigureBtn.Click
        ConfigureGrid.Visibility = Visibility.Visible
        ToolsTwoGrid.Visibility = Visibility.Collapsed
    End Sub

    Private Sub CloseConfigureBtn_Click(sender As Object, e As RoutedEventArgs) Handles CloseConfigureBtn.Click
        ConfigureGrid.Visibility = Visibility.Collapsed
    End Sub
    '   GUEST MGMT
    Private Sub GuestMgmtBtn_Click(sender As Object, e As RoutedEventArgs) Handles GuestMgmtBtn.Click
        HideAllGrids()
        GuestMgmtGrid.Visibility = Visibility.Visible
    End Sub

    Private Sub CloseGuestMenuBtn_Click(sender As Object, e As RoutedEventArgs) Handles CloseGuestMenuBtn.Click
        GuestMgmtGrid.Visibility = Visibility.Collapsed
    End Sub

    Private Sub AddNewGuestBtn_Click(sender As Object, e As RoutedEventArgs) Handles AddNewGuestBtn.Click

    End Sub

    Private Sub RemoveGuestBtn_Click(sender As Object, e As RoutedEventArgs) Handles RemoveGuestBtn.Click

    End Sub


End Class
