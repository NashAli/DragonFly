'   Copyright(c) Nash Ali/ETMS/NAE/NAP 2018
'    The MIT License(MIT)
'   Permission Is hereby granted, free Of charge, to any person obtaining a copy
'   of this software And associated documentation files(the "Software"), to deal
'   in the Software without restriction, including without limitation the rights
'   to use, copy, modify, merge, publish, distribute, sublicense, And / Or sell
'   copies of the Software, And to permit persons to whom the Software Is
'   furnished to do so, subject to the following conditions :
'   The above copyright notice And this permission notice shall be included In
'   all copies Or substantial portions Of the Software.

'   THE SOFTWARE Is PROVIDED "AS IS", WITHOUT WARRANTY Of ANY KIND, EXPRESS Or
'   IMPLIED, INCLUDING BUT Not LIMITED To THE WARRANTIES Of MERCHANTABILITY,
'   FITNESS FOR A PARTICULAR PURPOSE And NONINFRINGEMENT.IN NO EVENT SHALL THE
'   AUTHORS Or COPYRIGHT HOLDERS BE LIABLE For ANY CLAIM, DAMAGES Or OTHER
'   LIABILITY, WHETHER In AN ACTION Of CONTRACT, TORT Or OTHERWISE, ARISING FROM,
'   OUT OF Or IN CONNECTION WITH THE SOFTWARE Or THE USE Or OTHER DEALINGS IN
'   THE SOFTWARE.
'
'
'   CODE INDEX
'
'   00  It all starts here, all the required init code.
'   01  MESSAGE CENTRE MANAGER
'   02  USB STUFF
'   03  VISION & ROBOT SYSTEM
'   04  SYSTEM SOUNDS HANDLER
'   05  USER INPUT DIALOGUE SUPPORT CODE
'   06  Fingerprint biometrics
'   07  system data stuff
'   08  ALL MENUS
'   09  ALL THE WIFI SUPPORT HERE
'   10  BLUETOOTH SUPPORT CODE
'   11  HOME AUTOMATION SUPPORT CODE
'   12  ALL IO setup - I2C SPI UART
'   13  Filer Stuff  Media Stuff...
'   14  Network and Resident Login Stuff
'   15  GEO LOCATION
'   16  All Radios
'   17  System stuff
'   18  CLOCKS AND TIMERS
'   19  GRIDS STUFF - RESTORE MAIN
'   20  Speech Recognition and Synthesis and Audio
'   21  AUDIO PLAYER
'   22  COMPASS STUFF
'   23  BROWSER STUFF
'   24  GUEST MGMT SUB COMMANDS
'   25  admin sub-grid stuff
'   26
'   27
'   28
'   29
'   30
'   31
'   32
'   33
'   34
'   35
'   36
'   37
'   38
'   39



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
Imports System.Collections.ObjectModel
Imports Windows.Devices.Bluetooth.Rfcomm
Imports Windows.Phone
Imports System.IO
Imports System.Collections.Generic
Imports System
Imports Windows.UI.Xaml.Controls
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
Imports System.Reflection
Imports Windows.UI.Popups
Imports Windows.Devices.SerialCommunication
Imports System.Threading
Imports Windows.Graphics.Imaging
Imports Windows.Media.Capture
Imports Windows.System.Display
Imports Windows.Media.MediaProperties
Imports Windows.Media.Playback

Public NotInheritable Class MainPage
    Inherits Page
    '   This is for the calendar unit

    Friend Class Bookings
        Friend Property HasOpenings As DateTimeOffset
        Friend Property Booked

    End Class

    Private Class Residents
        Friend FirstName As String
        Friend LastName As String
        Friend Passcode As Integer
        Friend FPrint As BitmapImage
        Friend HShot As BitmapImage
        Friend Security As Integer
    End Class

    Private Class AllGuests
        Friend FirstName As String
        Friend LastName As String
        Friend Passcode As Integer
        Friend FPrint As BitmapImage
        Friend HShot As BitmapImage
        Friend Security As Integer
    End Class

    Private Class AllStaff
        Friend FirstName As String
        Friend LastName As String
        Friend Passcode As Integer
        Friend FPrint As BitmapImage
        Friend HShot As BitmapImage
        Friend Security As Integer
    End Class

    '   People DB
    Dim PAYLOAD As Integer, RESPONSE As Integer
    Dim EventLogDB(logTime, USERID, Pcode)
    Dim UserNumber As Integer
    Dim Security As Byte
    Dim Uphoto As BitmapBuffer
    Dim USERID As Integer
    Dim UserName As String
    Dim TotalUsers As Integer
    Dim logTime As String
    Dim Pcode As Integer




    '   00
    '   ******  It All Starts Here  *************************************************
    Private Sub App_Closing() Handles Me.Unloaded
        captureManager.Dispose()
    End Sub
    Private Sub MainPage_Ready() Handles Me.Loaded
        Start_DeviceWatch()
        InitDragonfly()
        ShowDragonfly()
        StartDragonfly()
    End Sub
    '   init dragonfly
    Private Sub InitDragonfly()
        PIRSensorMode.IsOn = False
        InitializeSerialPort()
        Init_MOTION()
        SetQueryOptions()
        Dim eas As New EasClientDeviceInformation()
        MainTitleTxt.Text = "DragonFly: " + eas.SystemProductName
        StartClockTimers()
        HideAllGrids()
        RestoreMain()
        LoadBookmarks()
        MyPlayer.Volume = 0.1
        MainVolumeSlider.Value = 0.1
        Talker(David, IntroText)
        VoiceSelector.Items.Clear()
        Dim list = From a In SpeechSynthesizer.AllVoices
        For Each voice In list
            VoiceSelector.Items.Add(voice.DisplayName)
        Next
        InitCompass()
        SetCompassOperatingMode(CompassOperatingMode.CONTINUOUS_OPERATING_MODE)
        PassMessage("Welcome", "Dragonfly has restarted")
    End Sub
    '   show dragonfly
    Private Sub ShowDragonfly()
        RestoreMain()
    End Sub
    '   Start DragonFly - Jump to user App and back!
    Private Sub StartDragonfly()
        'more stuff
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
    '   **********  END


    '   01
    '   MESSAGE CENTRE MANAGER  ***********************************************

    Dim MessTitle(), MessBody() As String
    Dim MessageCentreAlert As Boolean = False
    Dim QueuedMessages As Integer = 0

    '   Pass it on.. add new message to top of stack
    Private Sub PassMessage(title As String, body As String)
        QueuedMessages = QueuedMessages + 1
        MessTitle(QueuedMessages) = title
        MessBody(QueuedMessages) = body
        MessageCentreAlert = True
    End Sub

    '   Show the message at #1 in the stack....
    Private Sub ShowMessage()
        If MessageCentreAlert Then
            NoFMessTxt.Text = "Queued Messages: " + QueuedMessages.ToString
            GeneralStatusTxt.Text = MessBody(1)
            StatusTitleTxt.Text = MessTitle(1)
            StatusGrid.Visibility = Visibility.Visible
        End If
    End Sub

    '   Dismiss the first message at bottom of stack
    Private Sub HideMessage(ByVal sender As Object, ByVal args As TappedRoutedEventArgs) Handles StatusGrid.Tapped
        StatusGrid.Visibility = Visibility.Collapsed
        ShuffleStack()
    End Sub

    Private Sub ShuffleStack()
        '   shuffle the messages down the queue
        Dim newtop = QueuedMessages - 1
        For x = 1 To newtop
            MessBody(x) = MessBody(x + 1)
        Next x
        QueuedMessages = QueuedMessages - 1
        If QueuedMessages < 1 Then
            QueuedMessages = 0
            MessageCentreAlert = False
        End If
    End Sub
    '   CHECK FOR NEW MESSAGE
    Private Sub Check_Message()
        If MessageCentreAlert Then
            ShowMessage()
        End If
    End Sub


    '   02   ***************************************************************************************************
    '   USB STUFF  
    Private Sub ScanUSB()
        UsbConnectedDevicesList.Items.Clear()
        ScanUSBCamera()
        ScanUSBSpeaker()
        ScanUSBMic()
        ScanUSBStorageMemory()
    End Sub

    Private Async Sub ScanUSBCamera()
        Dim AqStrFilter As String = DeviceInformationKind.Device
        Try
            Dim UsbDevices = Await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)
            If UsbDevices.Count > 0 Then
                PassMessage("USB Camera:", "Devices found")
                For Each device In UsbDevices
                    UsbConnectedDevicesList.Items.Add(device.Name)
                Next
            Else
                UsbConnectedDevicesList.Items.Add("no camera attached")
            End If
        Catch ex As Exception
            PassMessage("USB Camera:", "error getting devices")
        End Try
    End Sub
    Private Async Sub ScanUSBSpeaker()
        Dim AqStrFilter As String = DeviceInformationKind.Device
        Try
            Dim UsbDevices = Await DeviceInformation.FindAllAsync(DeviceClass.AudioRender)
            If UsbDevices.Count > 0 Then
                PassMessage("USB Speaker:", "Devices found")
                For Each device In UsbDevices
                    UsbConnectedDevicesList.Items.Add(device.Name)
                Next
            Else
                UsbConnectedDevicesList.Items.Add("no speaker plugged in")
            End If
        Catch ex As Exception
            PassMessage("USB Speaker:", "error getting devices")
        End Try
    End Sub
    Private Async Sub ScanUSBMic()
        Dim AqStrFilter As String = DeviceInformationKind.Device
        Try
            Dim UsbDevices = Await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture)
            If UsbDevices.Count > 0 Then
                PassMessage("USB Audio:", "Devices found")
                For Each device In UsbDevices
                    UsbConnectedDevicesList.Items.Add(device.Name)
                Next
            Else
                UsbConnectedDevicesList.Items.Add("no microphone plugged in")
            End If
        Catch ex As Exception
            PassMessage("USB Audio:", "error getting devices")
        End Try
    End Sub
    Private Async Sub ScanUSBStorageMemory()
        Dim AqStrFilter As String = DeviceInformationKind.Device
        Try
            Dim UsbDevices = Await DeviceInformation.FindAllAsync(DeviceClass.PortableStorageDevice)
            If UsbDevices.Count > 0 Then
                PassMessage("USB Storage:", "Devices found")
                For Each device In UsbDevices
                    UsbConnectedDevicesList.Items.Add(device.Name)
                Next
            Else
                UsbConnectedDevicesList.Items.Add("no storage plugged in")
            End If
        Catch ex As Exception
            PassMessage("USB Storage:", "error getting devices")
        End Try
    End Sub
    '   ***********************************************************************
    '   USB Device watcher
    Public Property Usbwatcher

    Private Sub Start_DeviceWatch()
        PassMessage("USB Watch: ", "Starting watch...")
        Dim Usbwatcher = DeviceInformation.CreateWatcher(DeviceClass.All)
        AddHandler Usbwatcher.Added, AddressOf New_Usb_Device
        Usbwatcher.Start()
    End Sub
    '   New device was plugged in...
    Private Async Sub New_Usb_Device()
        Dim AqStrFilter As String = DeviceInformationKind.Device
        Dim UsbDevices = Await DeviceInformation.FindAllAsync()
        PassMessage("USB Watch:", "New Devices found")
    End Sub
    '   End USB Device watch
    Private Sub End_DeviceWatch()
        PassMessage("USB Watch:", "ending watch..")
        Usbwatcher.Stop()
    End Sub
    '   ***********************************************************************



    '   03
    '   ***********************************************************************
    '   VISION & ROBOT SYSTEM 
    Dim ONE_SEC As Integer = 0
    Private photoFile As StorageFile
    Private recordStorageFile As StorageFile
    Dim ROBOT18_AUTO As Boolean = False
    Dim MOTORXPOS As Integer = 0
    Dim MOTORYPOS As Integer = 0
    Dim MOTORZPOS As Integer = 0
    Dim captureManager As MediaCapture = New MediaCapture()
    Private ReadOnly displayRequest As DisplayRequest = New DisplayRequest()
    Private isVideoInitialized As Boolean = False
    Private isVideoPreviewing As Boolean = False
    Private isVideoRecording As Boolean = False

    '   start preview
    Private Async Sub Start_Preview()
        '   show data and preview window
        VisionGrid.Visibility = Visibility.Visible
        If Not isVideoInitialized Then
            Init_Camera()
        End If
        Try
            '   start the preview
            capturePreview.Source = captureManager
            Await captureManager.StartPreviewAsync()
            isVideoPreviewing = True
            PassMessage("CAMERA:", "Preview Starting...")
        Catch ex As Exception
            isVideoPreviewing = False
            PassMessage("CAMERA:", "Preview Error - Camera not initialized!")
        End Try
    End Sub
    '   stop preview
    Private Async Sub Stop_Preview()
        If isVideoPreviewing Then
            Try
                '   stop the preview
                Await captureManager.StopPreviewAsync()
                '   hide the vision and data window
                VisionGrid.Visibility = Visibility.Collapsed
                PassMessage("CAMERA:", "Preview Stopped...")
                isVideoPreviewing = False
            Catch ex As Exception
                PassMessage("CAMERA:", "Preview Stop Fail!")
            End Try
        End If
    End Sub


    Private Sub ActivateVision_Click(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles ActivateVisionBtn.Click
        RobotGrid.Visibility = Visibility.Collapsed
        Open_Camera()
    End Sub

    Private Sub Robot_Click(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles RobotBtn.Click
        HideAllGrids()
        RobotGrid.Visibility = Visibility.Visible
    End Sub
    Private Sub Init_Robot()
        RobotMotorGrid.Visibility = Visibility.Visible
        X_MotorTxt.Text = "X Motor: no value"
        Y_MotorTxt.Text = "Y Motor: no value"
        Z_MotorTxt.Text = "Z Motor: no value"
    End Sub
    Private Sub StartRobot_Click(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles RobotStartBtn.Click
        Init_Robot()
        PassMessage("ROBOT:", "Robot has Started")
    End Sub
    Private Sub EmStopRobot_Click(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles RobotEStopBtn.Click
        RobotMotorGrid.Visibility = Visibility.Collapsed
        PassMessage("ROBOT:", "Robot has Stopped")
    End Sub


    '   close camera
    Private Sub Close_Camera()
        captureManager.Dispose()
        isVideoInitialized = False
        VisionGrid.Visibility = Visibility.Collapsed
    End Sub
    '   open camera
    Private Sub Open_Camera()
        If Not isVideoInitialized Then
            Init_Camera()
        End If
        Start_Preview()
    End Sub

    Private Function GenerateNewFileName(ByVal Optional prefix As String = "IMG") As String
        Return prefix & "_" + DateTime.UtcNow.ToString("yyyy-MMM-dd_HH-mm-ss")
    End Function

    '   Initialize the camera
    Private Async Sub Init_Camera()
        Try
            ' Get available devices for capturing pictures
            Dim allVideoDevices = Await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)
            Dim desiredDevice As DeviceInformation = allVideoDevices.FirstOrDefault
            Dim settings As New MediaCaptureInitializationSettings() With
                {.SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                .MemoryPreference = MediaCaptureMemoryPreference.Auto,
                .StreamingCaptureMode = StreamingCaptureMode.Video}
            Await captureManager.InitializeAsync(settings)
            isVideoInitialized = True
            PassMessage("CAMERA:", "Camera Init. OK!")
        Catch ex As Exception
            isVideoInitialized = False
            PassMessage("CAMERA:", "Camera Not Found!")
        End Try
    End Sub

    Private Sub Close_Vision(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles CloseCameraABB.Click
        Stop_Preview()
        VisionGrid.Visibility = Visibility.Collapsed
    End Sub

    Private Sub ImageCapture_Click(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles CaptureImageABB.Click
        Capture_PhotoAsync()
    End Sub
    Private Sub VideoCapture_Click(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles CaptureClipABB.Click
        Capture_Clip()
    End Sub
    Private Async Sub Capture_PhotoAsync()
        '       grab a photo image.
        Await captureManager.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), photoFile)
        '       save image.
        photoFile = Await KnownFolders.PicturesLibrary.CreateFileAsync(GenerateNewFileName, CreationCollisionOption.GenerateUniqueName)
    End Sub

    Private Async Sub Capture_Clip()
        ONE_SEC = 0
        Await captureManager.StartRecordToStreamAsync(MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto), recordStorageFile)
        While ONE_SEC < 10
            CapTimerTxt.Text = ONE_SEC.ToString
        End While
        Await captureManager.StopRecordAsync
        Await KnownFolders.VideosLibrary.CreateFileAsync(GenerateNewFileName)
        '       save as a mp4 file.
    End Sub



    '   04
    '   SYSTEM SOUNDS HANDLER   ***********************************************
    Dim SelectedSound As StorageFile

    Private Async Sub PlaySound(snd)
        '   set query to "snd"
        Dim MyBaseUri = "ms-appx:///Sounds/"
        MyBaseUri = MyBaseUri + snd + ".wav"
        SelectedSound = Await StorageFile.GetFileFromApplicationUriAsync(New Uri(MyBaseUri))
        MyPlayer.AutoPlay = False
        MyPlayer.SetSource(MyBackgroundAudioStream, SelectedSound.ContentType)
        MyPlayer.Play()
    End Sub

    '   05
    '   USER INPUT DIALOGUE SUPPORT CODE    ***********************************
    Public UserTextOK As Boolean = False
    Public UserResponseCancelled As Boolean = False
    Public UserResponseText As String = Nothing
    Public UserMessageText As String = Nothing

    Public Sub UserResponse(UserMessageText)

        UserMessage.Text = UserMessageText
        UserResponseText = Nothing
        UserResponseCancelled = False
        UserTextOK = False
        GetUserTextDialogueGrid.Visibility = Visibility.Visible
        Do While Not UserTextOK
        Loop
        Dim response = UserResponseText

    End Sub
    Private Sub UserDialogueCancelBtn_Click(sender As Object, e As RoutedEventArgs) Handles UserDialogueCancelBtn.Click
        UserResponseCancelled = True
        UserTextOK = True
        GetUserTextDialogueGrid.Visibility = Visibility.Collapsed
        UserResponseText = ""
        PassMessage("User Dialog:", "Operation Cancelled!")
    End Sub
    Private Sub UserTextOKBtn_Click(sender As Object, e As RoutedEventArgs) Handles UserTextOKBtn.Click
        If UserResponseText IsNot "" Then
            UserTextOK = True
            UserResponseCancelled = False
        ElseIf UserResponseText Is "" Then
            UserTextOK = True
            UserResponseCancelled = True
        End If
        GetUserTextDialogueGrid.Visibility = Visibility.Collapsed
    End Sub

    '   06
    '   ***************************************************************************************************************
    '   Fingerprint biometrics
    '   *****************************************
    '   serial fingerprint i/f
    '   All of the functions of the Fingerprint Module
    '   Enters the resident/guest entry/verification system
    '   this uses the UART Fingerprint Reader module.
    '   Some basic support code...

    Dim ReceiveData() As Byte
    Private serialPort As SerialDevice = Nothing
    Private MyDataReader As DataReader = Nothing
    Private DataWriterObject As DataWriter = Nothing
    Private listOfDevices As ObservableCollection(Of DeviceInformation)
    Private ReadCancelTokenSource As CancellationTokenSource
    Dim CommandTransmitDataBuffer(7) As Byte, CommandValue As Byte, User As Integer, Query As Boolean
    Dim StatusReceiveDataBuffer(7) As Byte
    Private Const prepostamble As Byte = &HF5
    Private Const zero As Byte = &H0
    Private Const one As Byte = &H1
    Public EigenVal = 197

    Private Sub FingerprintBtn_Click(sender As Object, e As RoutedEventArgs) Handles FingerprintBtn.Click
        HideAllGrids()
        FingerprintGrid.Visibility = Visibility.Visible
        CheckTotalUsers()
    End Sub


    Enum FingerPrintResponse
        Success = &H0
        Fail = &H1
        Full = &H4
        NoUser = &H5
        UserExists = &H6
        PrintExists = &H7
        AckTimeout = &H8
    End Enum

    Enum FingerPrintCommand
        CMD_REG_START_DB = &H1
        CMD_REG_SECOND_DB = &H2
        CMD_REG_END_DB = &H3
        CMD_REG_DELETE_DB = &H4
        CMD_REG_ALLDEL_DB = &H5
        CMD_REG_UPLOAD_DB = &H6
        CMD_GET_USER_NUMBER_DB = &H8
        CMD_GET_USER_SUM_DB = &H9
        CMD_GET_USER_RIGHT_DB = &HA
        CMD_VERIFY_DB = &HB
        CMD_IDENTIFY_DB = &HC
        CMD_LUM_ADJUST = &HF
        CMD_SET_BAUD = &H21
        CMD_SET_REG = &H22
        CMD_GET_VALUE = &H23
        CMD_GET_IMAGE = &H24
        CMD_TEST_COMM = &H25
        CMD_GET_VERSION = &H26
        CMD_SET_MATCH_LEVEL = &H28
        CMD_PROCESS_IMAGE = &H29
        CMD_GET_USER_INFO = &H2B
        CMD_SET_LP_MODE = &H2C
        CMD_SET_ENROLL_MODE = &H2D
        CMD_SET_ENROLL_TIMEOUT = &H2E
        CMD_GET_LUM = &H2F
        CMD_UPLOAD__VALUE_DB = &H31
        CMD_UPLOAD_ID_UNLOCK = &H32
        CMD_UPLOAD_RAND_UNLOCK = &H34
        CMD_FROM_VALUE_DB = &H41
        CMD_FROM_VERIFY_DB = &H42
        CMD_FROM_IDENTIFY_DB = &H43
        CMD_FROM_VERIFY = &H44
        CMD_FROM_VERIFY_2 = &H46
        CMD_DNLOAD_RAND_UNLOCK = &H48
        CMD_PROG_UPGRADE = &HFF
    End Enum

    Enum SecurityClearance
        Low = 1
        Medium = 2
        High = 3
    End Enum

    Public Enum FileFormat
        Jpeg
        Png
        Bmp
        Tiff
        Gif
    End Enum


    Public Async Function CheckFingerprintAvailability() As Task(Of String)
        Dim returnMessage As String = ""
        Try
            Dim ucvAvailability = Await UI.UserConsentVerifier.CheckAvailabilityAsync()
            Select Case ucvAvailability
                Case FingerPrintResponse.Success
                    returnMessage = "Fingerprint verification is available."
                Case FingerPrintResponse.AckTimeout
                    returnMessage = "Biometric device is busy."
                Case FingerPrintResponse.Fail
                    returnMessage = "No biometric device found."
                Case UI.UserConsentVerifierAvailability.DisabledByPolicy
                    returnMessage = "Biometric verification is disabled by policy."
                Case FingerPrintResponse.NoUser
                    returnMessage = "The user has no fingerprints registered. Please add a fingerprint to the " & "fingerprint database and try again."
                Case Else
                    returnMessage = "Fingerprints verification is currently unavailable."
            End Select
        Catch ex As Exception
            returnMessage = "Fingerprint authentication availability check failed: " & ex.ToString()
        End Try

        Return returnMessage
    End Function
    Private Async Function RequestConsent(ByVal userMessage As String) As System.Threading.Tasks.Task(Of String)
        Dim returnMessage As String
        If String.IsNullOrEmpty(userMessage) Then
            userMessage = "Please provide fingerprint verification."
        End If

        Try
            Dim consentResult = Await UI.UserConsentVerifier.RequestVerificationAsync(userMessage)
            Select Case consentResult
                Case FingerPrintResponse.Success
                    returnMessage = "Fingerprint verified."
                Case FingerPrintResponse.Fail
                    returnMessage = "Biometric device is busy."
                Case UI.UserConsentVerificationResult.DeviceNotPresent
                    returnMessage = "No biometric device found."
                Case UI.UserConsentVerificationResult.DisabledByPolicy
                    returnMessage = "Biometric verification is disabled by policy."
                Case FingerPrintResponse.NoUser
                    returnMessage = "The user has no fingerprints registered. Please add a fingerprint to the " & "fingerprint database and try again."
                Case UI.UserConsentVerificationResult.RetriesExhausted
                    returnMessage = "There have been too many failed attempts. Fingerprint authentication canceled."
                Case UI.UserConsentVerificationResult.Canceled
                    returnMessage = "Fingerprint authentication canceled."
                Case Else
                    returnMessage = "Fingerprint authentication is currently unavailable."
            End Select
        Catch ex As Exception
            returnMessage = "Fingerprint authentication failed: " & ex.ToString()
        End Try

        Return returnMessage
    End Function




    Public Shared Function ByteToHexString(B As Byte) As String
        Const hex As String = "0123456789ABCDEF"
        Dim lowNibble As Integer = B And &HF
        Dim highNibble As Integer = (B And &HF0) >> 4
        Dim s As New String(New Char() {hex(highNibble), hex(lowNibble)})
        Return s
    End Function
    '   Assemble FP Module command
    Private Sub AssembleCommand(CommandValue As Byte, User As Integer, Query As Boolean)
        CommandTxt.Text = ""
        Try
            CommandTransmitDataBuffer(0) = prepostamble   'var 1
            CommandTransmitDataBuffer(1) = CommandValue '   command hex code; see specs.
            CommandTransmitDataBuffer(2) = zero        '   zero for now high order user byte
            CommandTransmitDataBuffer(3) = User     ' 1 to 255 this will represent one finger out of ten
            If Query Then
                CommandTransmitDataBuffer(4) = one        '   1 for query and 0 to set data
            ElseIf Not Query Then
                CommandTransmitDataBuffer(4) = zero
            End If
            CommandTransmitDataBuffer(5) = Security        '  security level
            CommandTransmitDataBuffer(6) = CommandTransmitDataBuffer(1) Xor CommandTransmitDataBuffer(2) Xor CommandTransmitDataBuffer(3) Xor CommandTransmitDataBuffer(4) Xor CommandTransmitDataBuffer(5) 'var 7 from 2 to 6
            CommandTransmitDataBuffer(7) = prepostamble   'var 8
            For x = 0 To 6
                CommandTxt.Text = CommandTxt.Text + ByteToHexString(CommandTransmitDataBuffer(x)) + " - "
            Next
            CommandTxt.Text = CommandTxt.Text + ByteToHexString(CommandTransmitDataBuffer(7))
            portStatus.Text = "Assembly Complete!"
        Catch ex As Exception
            portStatus.Text = "Assembly Fail!"
        End Try
    End Sub


    'Convert byte stream to image
    Private Sub ConvertStreamToImage()

        Dim ImgBuffer As New WriteableBitmap(124, 148)
        'UserPrintImage is the destination imageholder - 9,176 bytes 124 x 148 4-bit grayscale
        'Eigenvalues data length Len-3 is fixed 193 bytes.
        For x = 1 To 124
            For y = 1 To 148
                'transfer the byte in the buffer to the two pixels low-pixel first ??
                'UserPrintImage

            Next
        Next
    End Sub
    Public Shared Async Function StorageFileToBitmapImage(savedStorageFile As StorageFile) As Task(Of BitmapImage)
        Using fileStream As IRandomAccessStream = Await savedStorageFile.OpenAsync(FileAccessMode.Read)
            Dim UserPrintImage As New BitmapImage()
            UserPrintImage.DecodePixelHeight = 148
            UserPrintImage.DecodePixelWidth = 124
            Await UserPrintImage.SetSourceAsync(fileStream)
            Return UserPrintImage
        End Using
    End Function
    Public Shared Async Function WriteableBitmapToStorageFile(WB As WriteableBitmap, fileFormat__1 As FileFormat) As Task(Of StorageFile)
        Dim FileName As String = "MyFile."
        Dim BitmapEncoderGuid As Guid = BitmapEncoder.JpegEncoderId
        Select Case fileFormat__1
            Case FileFormat.Jpeg
                FileName += "jpeg"
                BitmapEncoderGuid = BitmapEncoder.JpegEncoderId
                Exit Select
            Case FileFormat.Png
                FileName += "png"
                BitmapEncoderGuid = BitmapEncoder.PngEncoderId
                Exit Select
            Case FileFormat.Bmp
                FileName += "bmp"
                BitmapEncoderGuid = BitmapEncoder.BmpEncoderId
                Exit Select
            Case FileFormat.Tiff
                FileName += "tiff"
                BitmapEncoderGuid = BitmapEncoder.TiffEncoderId
                Exit Select
            Case FileFormat.Gif
                FileName += "gif"
                BitmapEncoderGuid = BitmapEncoder.GifEncoderId
                Exit Select
        End Select
        Dim file = Await ApplicationData.Current.TemporaryFolder.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName)
        Using stream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.ReadWrite)
            Dim encoder As BitmapEncoder = Await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream)
            Dim pixelStream As Stream = WB.PixelBuffer.AsStream()
            Dim pixels As Byte() = New Byte(pixelStream.Length - 1) {}
            Await pixelStream.ReadAsync(pixels, 0, pixels.Length)
            encoder.SetPixelData(BitmapPixelFormat.Gray8, BitmapAlphaMode.Ignore, CUInt(WB.PixelWidth), CUInt(WB.PixelHeight), 96.0, 96.0,
                pixels)
            Await encoder.FlushAsync()
        End Using
        Return file
    End Function
    '   test the interface
    '   Here is my take on the onboard serial communications being used with a fingerprint scanner
    '   Initialize the serialport
    Public Async Sub InitializeSerialPort()
        Try
            Dim aqs As String = SerialDevice.GetDeviceSelector("UART0")
            Dim dis = Await DeviceInformation.FindAllAsync(aqs)
            serialPort = Await SerialDevice.FromIdAsync(dis(0).Id)
            serialPort.WriteTimeout = TimeSpan.FromMilliseconds(2000)
            serialPort.ReadTimeout = TimeSpan.FromMilliseconds(4000)
            serialPort.BaudRate = 19200
            serialPort.Parity = SerialParity.None
            serialPort.StopBits = SerialStopBitCount.One
            serialPort.DataBits = 8
            MyDataReader = New DataReader(serialPort.InputStream) With {
                .InputStreamOptions = InputStreamOptions.[Partial]
            }
            DataWriterObject = New DataWriter(serialPort.OutputStream)
            ReadCancelTokenSource = New CancellationTokenSource()
            portStatus.Text = "UART0 Initialize OK"
            PassMessage("FINGERPRINT MODULE", "UART0 Initialize OK")
            StartReceive()
        Catch ex As Exception
            portStatus.Text = "UART0 Initialize Error" + ex.ToString
            PassMessage("FINGERPRINT MODULE", "UART0 Initialize Error" + ex.ToString)
        End Try
    End Sub
    '   Start the Receive process
    Public Async Sub StartReceive()
        While True
            Await Listen()
            If (ReadCancelTokenSource.Token.IsCancellationRequested) OrElse (serialPort Is Nothing) Then
                Exit While
            End If
        End While
    End Sub
    'LISTEN FOR NEXT RECEIVE
    Private Async Function Listen() As Task
        RecStatusTxt.Text = "waiting ..."
        RESPONSE = 8
        PAYLOAD = 0
        Dim count = RESPONSE + PAYLOAD
        Dim bytesRead As UInt32
        Try
            If serialPort IsNot Nothing Then
                While True
                    bytesRead = Await MyDataReader.LoadAsync(count).AsTask()
                    'Wait until buffer is full
                    If (ReadCancelTokenSource.Token.IsCancellationRequested) OrElse (serialPort Is Nothing) Then
                        Exit While
                    End If

                    If bytesRead > 0 And bytesRead < 9 Then
                        BytesReadTxt.Text = "BYTES READ: " + bytesRead.ToString
                        RecStatusTxt.Text = "received status..."
                        ModuleStatusTxt.Text = ""
                        For x = 0 To 6
                            StatusReceiveDataBuffer(x) = MyDataReader.ReadByte()
                            ModuleStatusTxt.Text = ModuleStatusTxt.Text + ByteToHexString(StatusReceiveDataBuffer(x)) + " - "
                        Next
                        ModuleStatusTxt.Text = ModuleStatusTxt.Text + ByteToHexString(StatusReceiveDataBuffer(7))
                        PAYLOAD = (StatusReceiveDataBuffer(2) * 256) + StatusReceiveDataBuffer(3)
                        ReceiveData = New Byte(PAYLOAD - 1) {}
                        MyDataReader.ReadBytes(ReceiveData)
                        For Each Data As Byte In ReceiveData
                            'foreach (byte Data in ReceiveData)
                            rcvdPayloadTxt.Text = rcvdPayloadTxt.Text + ByteToHexString(Data) + " - "
                        Next
                    End If
                    If bytesRead > 8 Then
                        BytesReadTxt.Text = "BYTES READ: " + bytesRead.ToString
                        RecStatusTxt.Text = "received payload"
                        ModuleStatusTxt.Text = ""
                        For x = 0 To 6
                            StatusReceiveDataBuffer(x) = MyDataReader.ReadByte()
                            ModuleStatusTxt.Text = ModuleStatusTxt.Text + ByteToHexString(StatusReceiveDataBuffer(x)) + " - "
                        Next
                        ModuleStatusTxt.Text = ModuleStatusTxt.Text + ByteToHexString(StatusReceiveDataBuffer(7))

                        For Each Data As Byte In ReceiveData
                            'foreach (byte Data in ReceiveData)
                            rcvdPayloadTxt.Text = rcvdPayloadTxt.Text + ByteToHexString(Data) + " - "
                        Next
                    End If
                End While
            End If
        Catch ex As Exception
            If ReadCancelTokenSource IsNot Nothing Then
                ReadCancelTokenSource.Cancel()
            End If
            portStatus.Text = "UART ReadAsync Exception: {0}"
        End Try
    End Function
    '********** SEND BYTES **********
    '
    '
    Public Async Sub SendBytes(TxData As Byte())
        Try
            'Send data to UART
            DataWriterObject.WriteBytes(TxData)
            Await DataWriterObject.StoreAsync()
            'portStatus.Text = "bytes sent..."
        Catch ex As Exception
            PassMessage("FPM:", "UART0 Tx Err" + ex.ToString)
        End Try
    End Sub

    '   Get total count of the number of users in the module
    Private Sub CheckTotalUsers()
        AssembleCommand(FingerPrintCommand.CMD_GET_USER_SUM_DB, 0, 0)
        SendBytes(CommandTransmitDataBuffer)
        StartReceive()
        If StatusReceiveDataBuffer(FingerPrintResponse.Success) Then
            TotalUsers = ((StatusReceiveDataBuffer(3) * 256) + StatusReceiveDataBuffer(4))
            TotalUsersTxt.Text = "Total UsersDB : " + TotalUsers.ToString
        End If
    End Sub
    '   Add a new user
    Private Sub AddNewPrint()
        Try1(USERID)
        Try2(USERID)
        Try3(USERID)
    End Sub
    '   Log into the module 3 tries.
    Private Async Sub Try1(USERID)
        portStatus.Text = "1 of 3"
        While StatusReceiveDataBuffer(4) <> FingerPrintResponse.Success
            CommandTransmitDataBuffer(5) = Security
            AssembleCommand(FingerPrintCommand.CMD_REG_START_DB, USERID, 0)
            SendBytes(CommandTransmitDataBuffer)
            '   set PAYLOAD
            Await Listen()
        End While
    End Sub
    Private Async Sub Try2(USERID)
        portStatus.Text = "2 of 3"
        While StatusReceiveDataBuffer(4) <> FingerPrintResponse.Success
            CommandTransmitDataBuffer(5) = Security
            AssembleCommand(FingerPrintCommand.CMD_REG_SECOND_DB, USERID, 0)
            SendBytes(CommandTransmitDataBuffer)
            Await Listen()
        End While
    End Sub
    Private Async Sub Try3(USERID)
        portStatus.Text = "3 of 3"
        While StatusReceiveDataBuffer(4) <> FingerPrintResponse.Success
            CommandTransmitDataBuffer(5) = Security
            AssembleCommand(FingerPrintCommand.CMD_REG_END_DB, USERID, 0)
            SendBytes(CommandTransmitDataBuffer)
            Await Listen()
        End While
    End Sub
    '   Aquire Fingerprint
    Private Sub RequestPrint()
        Try
            PAYLOAD = 9176
            CommandTransmitDataBuffer(5) = Security
            AssembleCommand(FingerPrintCommand.CMD_GET_IMAGE, USERID, 0)
            SendBytes(CommandTransmitDataBuffer)
            StartReceive()
            'Await Listen()
        Catch ex As Exception

        End Try
    End Sub
    '   Verify FingerPrint
    Private Sub VerifyPrint()
        portStatus.Text = "Verify print..."
        Try
            CommandTransmitDataBuffer(5) = Security
            AssembleCommand(FingerPrintCommand.CMD_FROM_VERIFY, USERID, 0)
            SendBytes(CommandTransmitDataBuffer)
            StartReceive()
        Catch ex As Exception

        End Try
    End Sub
    '   Delete Module database
    Private Sub DeleteModuleDatabase()
        portStatus.Text = "Deleting DB..."
        Try
            CommandTransmitDataBuffer(5) = Security
            AssembleCommand(FingerPrintCommand.CMD_REG_ALLDEL_DB, USERID, 0)
            SendBytes(CommandTransmitDataBuffer)
            StartReceive()
        Catch ex As Exception

        End Try
    End Sub
    '   Get module status
    Private Sub GetModuleStatus()
        portStatus.Text = "Getting status..."
        Try
            CommandTransmitDataBuffer(5) = Security
            AssembleCommand(FingerPrintCommand.CMD_IDENTIFY_DB, USERID, 0)
            SendBytes(CommandTransmitDataBuffer)
            StartReceive()
        Catch ex As Exception

        End Try
    End Sub

    '   hide the fp grid
    Private Sub CloseFingerModBtn_Click(sender As Object, e As RoutedEventArgs) Handles CloseFingerModBtn.Click
        FingerprintGrid.Visibility = Visibility.Collapsed
        RestoreMain()
    End Sub
    '   Open Fing Menu
    Private Sub FingCommandMenuBtn_Click(sender As Object, e As RoutedEventArgs) Handles FingCommandMenuBtn.Click
        FingerprintCommandGrid.Visibility = Visibility.Visible
    End Sub
    '   Close Fing Command
    Private Sub CloseFCGridBtn_Click(sender As Object, e As RoutedEventArgs) Handles CloseFCGridBtn.Click
        FingerprintCommandGrid.Visibility = Visibility.Collapsed
    End Sub
    '   
    Private Sub AddPrintBtn_Click(sender As Object, e As RoutedEventArgs) Handles AddPrintBtn.Click
        FingerprintCommandGrid.Visibility = Visibility.Collapsed
        AddNewPrint()
    End Sub
    Private Sub GetPrintBtn_Click(sender As Object, e As RoutedEventArgs) Handles GetPrintBtn.Click
        FingerprintCommandGrid.Visibility = Visibility.Collapsed
        RequestPrint()
    End Sub
    Private Sub DeviceStatusBtn_Click(sender As Object, e As RoutedEventArgs) Handles DeviceStatusBtn.Click
        FingerprintCommandGrid.Visibility = Visibility.Collapsed
        GetModuleStatus()
    End Sub
    Private Sub DeleteDBBtn_Click(sender As Object, e As RoutedEventArgs) Handles DeleteDBBtn.Click
        FingerprintCommandGrid.Visibility = Visibility.Collapsed
        '   TODO add an "are you sure!!" here...
        DeleteModuleDatabase()
    End Sub






    '   07
    '   ***************************************************************************************************************
    '   system data stuff
    Private Async Sub RestoreData()
        Try
            'restores on restart, all current settings - sys.dat
            Dim storageFolder As StorageFolder = ApplicationData.Current.LocalFolder
            Dim sysdat As StorageFile = Await storageFolder.GetFileAsync("sys.dat")
        Catch ex As Exception
            PassMessage("ALERT!", "sysdat lost!")
        End Try
    End Sub
    Private Async Sub SaveData()
        Try
            'saves all current system settings file - sys.dat
            'Dim sysdat As StreamWriter
            ' Create sample file; replace if exists.
            Dim storageFolder As StorageFolder = ApplicationData.Current.LocalFolder
            Dim sysdat As StorageFile = Await storageFolder.CreateFileAsync("sys.dat", CreationCollisionOption.ReplaceExisting)
            Await FileIO.WriteTextAsync(sysdat, "ADMIN=0,PASS=0,BKDP=2")
        Catch ex As Exception
            PassMessage("ALERT!", "failed to write sysdat!")
        End Try
    End Sub
    '   update
    Private Sub UpdatePrefsBtn_Click(sender As Object, e As RoutedEventArgs) Handles UpdatePrefsBtn.Click
        SaveData()
    End Sub
    Private Sub SaveAllData()
        'SaveGuestDataBase()
        'SaveStaffDataBase()
        SaveData()
    End Sub





    '   ***************************************************************************************************************
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
        Talker(Zira, "This device will be shutting down for maintenance")
        ShutDown()
    End Sub
    Private Sub ReBootBtn_Click(sender As Object, e As RoutedEventArgs) Handles ReBootBtn.Click
        Talker(Zira, "rebooting in 10 seconds")
        ReBoot()
    End Sub
    Private Sub ShutDown()
        ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, TimeSpan.FromSeconds(8))
    End Sub
    Private Sub ReBoot()
        ShutdownManager.BeginShutdown(ShutdownKind.Restart, TimeSpan.FromSeconds(10))
    End Sub
    Private Sub CancelShutdown()
        ShutdownManager.CancelShutdown()
    End Sub
    '08
    '   ALL MENUS   ***************************************************************************************************
    '   ***********************************************************************
    '   from the main page
    Private Sub SystemInfoDisplayBtn_Click(sender As Object, e As RoutedEventArgs) Handles SystemInfoDisplayBtn.Click
        Select Case SystemInfoGrid.Visibility
            Case Visibility.Collapsed
                HideAllGrids()
                ShowSystemInfo()
                ShowNetworkStatus()
                CheckForWifiAdapters()
                ScanUSB()
                SystemInfoGrid.Visibility = Visibility.Visible
                Talker(Zira, "System Info")
                Exit Select
            Case Visibility.Visible
                RestoreMain()
        End Select
    End Sub
    '   Show SYSTEM info
    Private Sub ShowSystemInfo()
        GetMemory()
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
    '   Memory info
    Private Sub GetMemory()
        Dim memusage As Integer = CInt((MemoryManager.AppMemoryUsage / MemoryManager.AppMemoryUsageLimit) * 100)
        MemoryUsageInd.Value = memusage
        MemoryUsageTxt.Text = "Usage Level: " + MemoryManager.AppMemoryUsageLevel.ToString + " - " + memusage.ToString + " % used."
        Dim used As Integer = CInt(MemoryManager.AppMemoryUsage.ToString) / 1048576
        MemoryUsedTxt.Text = used.ToString + " MBytes"
    End Sub
    '   check for wifi adapters
    Private Async Sub CheckForWifiAdapters()
        Dim result = Await DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector())
        Dim access = Await WiFiAdapter.RequestAccessAsync()
        If result.Count >= 1 Then
            If access <> WiFiAccessStatus.Allowed Then
                InfoStatusTxt.Text = "WiFi Adapter! - No Access"

            Else
                InfoStatusTxt.Text = "WiFi Adapter! - Wifi Access"

            End If
        Else
            InfoStatusTxt.Text = "No WiFi Adapter!"
        End If
    End Sub



    '   TOOLS   ***************************************************************
    '   TOOLS1 MENU TOGGLE
    Private Sub ToolsDisplayBtn_Click(sender As Object, e As RoutedEventArgs) Handles ToolsDisplayBtn.Click
        Select Case ToolsMenuGrid.Visibility
            Case Visibility.Collapsed
                HideAllGrids()
                ToolsMenuGrid.Visibility = Visibility.Visible
                Exit Select
            Case Visibility.Visible
                ToolsMenuGrid.Visibility = Visibility.Collapsed
                RestoreMain()
        End Select
    End Sub
    '   TOOL1 MENU SUPPORT
    '   ADMIN STUFF
    Private Sub AdminBtn_Click(sender As Object, e As RoutedEventArgs) Handles AdminBtn.Click
        HideAllGrids()
        AdminGrid.Visibility = Visibility.Visible
    End Sub
    Private Sub AdminLockBtn_Click(sender As Object, e As RoutedEventArgs) Handles AdminLockBtn.Click

    End Sub
    '   Settings Sub-Grid Button
    Private Sub SettingsAccessBtn_Click(sender As Object, e As RoutedEventArgs) Handles SettingsAccessBtn.Click
        Talker(Zira, "User settings")
        ShowSettings()
    End Sub

    '   MUSIC MANAGER -  from the tools menu
    Private Sub AudioManagerBtn_Click(sender As Object, e As RoutedEventArgs) Handles AudioManagerBtn.Click
        HideAllGrids()
        AudioManagerGrid.Visibility = Visibility.Visible
        ShowAllMusic()
    End Sub
    '   Speech sub-grid *******
    Private Sub SpeechPlusBtn_Click(sender As Object, e As RoutedEventArgs) Handles SpeechPlusBtn.Click
        HideAllGrids()
        Talker(Zira, "Speech and AI interaction")
        SpeechPlusGrid.Visibility = Visibility.Visible
    End Sub
    '   Browser Sub-Grid
    Private Sub BrowserBtn_Click(sender As Object, e As RoutedEventArgs) Handles BrowserBtn.Click
        MyBrowser.Navigate(New Uri(BingHome))
        HideAllGrids()
        Talker(Zira, "Loading the home page, tap menu for more options")
        BrowserGrid.Visibility = Visibility.Visible
    End Sub
    '   GEO sub-grid
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
    '   Apps menu
    Private Sub AppsMenuBtn_ClickAsync(sender As Object, e As RoutedEventArgs) Handles AppsMenuBtn.Click
        Select Case IATitleGrid.Visibility
            Case Visibility.Visible
                RestoreMain()
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
    '   **********  HOME Automation sub-grid **********************************
    Private Sub AutomationBtn_Click(sender As Object, ByVal args As RoutedEventArgs) Handles AutomationBtn.Click
        HideAllGrids()
        GetAllScenes()
        AutoGrid.Visibility = Visibility.Visible
    End Sub
    '   Show the Clock grid
    Private Sub ClockCalendarShowBtn_Click(sender As Object, e As RoutedEventArgs) Handles ClockCalendarShowBtn.Click
        HideAllGrids()
        ClockGrid.Visibility = Visibility.Visible
        DateGrid.Visibility = Visibility.Visible
    End Sub
    '   Time grid double tapped
    Private Sub ClockGrid_DoubleTapped(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles ClockGrid.DoubleTapped
        HideAllGrids()
        TimePickerGrid.Visibility = Visibility.Visible
    End Sub
    Private Sub CancelAlarmTimeBtn_Click(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles CancelAlarmTimeBtn.Click
        TimePickerGrid.Visibility = Visibility.Collapsed
        RestoreMain()
        PassMessage("Alarm", "Cancelled!")
    End Sub
    Private Sub SetAlarmTimeBtn_Click(sender As Object, ByVal args As RoutedEventArgs) Handles SetAlarmTimeBtn.Click
        TimePickerGrid.Visibility = Visibility.Collapsed
        SetAlarmTime()
    End Sub
    Private Sub SetAlarmTime()
        '   set alarm
        RestoreMain()
        PassMessage("Alarm", "An alarm has been set!")
    End Sub
    '   Date grid double tapped,  show calendar sub-grid.
    Private Sub DateGrid_DoubleTapped(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles DateGrid.DoubleTapped
        HideAllGrids()
        CalendarGrid.Visibility = Visibility.Visible
    End Sub
    '   show weather  
    Private Sub ShowWeatherBtn_Click(sender As Object, ByVal args As RoutedEventArgs) Handles ShowWeatherBtn.Click
        ToolsMenuGrid.Visibility = Visibility.Collapsed
        WeatherGrid.Visibility = Visibility.Visible
        Talker(Zira, "Weather is always sunny")
    End Sub

    '   ***********************************************************************
    '   tool 2 menu
    Private Sub MoreToolsBtn_Click(sender As Object, e As RoutedEventArgs) Handles MoreToolsBtn.Click
        ToolsMenuGrid.Visibility = Visibility.Collapsed
        ToolsTwoGrid.Visibility = Visibility.Visible
    End Sub
    '   TOOL2 MENU SUPPORT
    Private Sub ConfigureBtn_Click(sender As Object, e As RoutedEventArgs) Handles ConfigureBtn.Click
        Talker(Zira, "Configuration menu")
        ConfigureGrid.Visibility = Visibility.Visible
        ToolsTwoGrid.Visibility = Visibility.Collapsed
    End Sub
    '   GUEST MGMT SUB GRID
    Private Sub GuestMgmtBtn_Click(sender As Object, e As RoutedEventArgs) Handles GuestMgmtBtn.Click
        HideAllGrids()
        GuestMgmtGrid.Visibility = Visibility.Visible
    End Sub
    '   SETTINGS sub-grid stuff
    Private Sub ShowSettings()
        HideAllGrids()
        SettingsGrid.Visibility = Visibility.Visible
        GetWallpaper()
    End Sub
    '   turns off settings grid
    Private Sub SettingsGridOKBtn_Click(sender As Object, e As RoutedEventArgs) Handles SettingsGridOKBtn.Click
        HideAllGrids()
        RestoreMain()
    End Sub

    '   09
    '   ***************************************************************************************************************
    '   ALL THE WIFI SUPPORT HERE
    '   TODO: CONNECT TO HOME AUTOMATION WIFI LINKS IF AVAILABLE
    '   
    '   wifi access connect grid
    Private Sub WifiAccessBtn_Click(ByVal sender As Object, e As RoutedEventArgs) Handles WifiAccessBtn.Click
        ShowAvailableWifiNetworks()
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
    End Sub
    Private Sub WifiConnectGridOKBtn_Click(sender As Object, e As RoutedEventArgs) Handles WifiConnectGridOKBtn.Click
        WifiConnectGrid.Visibility = Visibility.Collapsed
    End Sub
    Private Sub ConnectToNetworkBtn_Click(sender As Object, e As RoutedEventArgs) Handles ConnectToNetworkBtn.Click
        NetworkLoginGrid.Visibility = Visibility.Visible
        WifiConnectGrid.Visibility = Visibility.Collapsed
    End Sub



    '   10
    '   ***************************************************************************************************************
    '   BLUETOOTH SUPPORT CODE
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


    '   bluetooth manager selected 
    Private Sub BluetoothManagerBtn_Click(sender As Object, e As RoutedEventArgs) Handles BluetoothManagerBtn.Click
        HideAllGrids()
        BluetoothActivityGrid.Visibility = Visibility.Visible
        UpdateBluetoothDevices()
    End Sub
    '   close bluetooth manager menu
    Private Sub CloseBTMBtn_Click(sender As Object, e As RoutedEventArgs) Handles CloseBTMBtn.Click
        BluetoothActivityGrid.Visibility = Visibility.Collapsed
        RestoreMain()
    End Sub

    '   11
    '   ***************************************************************************************************************
    '   HOME AUTOMATION SUPPORT CODE
    Private Sub GetAllScenes()
        AllScenesCB.Items.Clear()
        AllScenesCB.Items.Add("default")
        AllScenesCB.Items.Add("away")
        AllScenesCB.Items.Add("test")
    End Sub

    Private Sub AddSceneBtn_Click(sender As Object, e As RoutedEventArgs) Handles AddSceneBtn.Click
        If UserTextOK Then
            AllScenesCB.Items.Add(item:=UserResponseText)
        End If
    End Sub
    Private Sub DeleteSceneBtn_Click(sender As Object, e As RoutedEventArgs) Handles DeleteSceneBtn.Click

    End Sub
    '   HOME AUTOMATION - HUE Emulation
    Private Sub HueEmulationSw_Toggled(sender As Object, e As RoutedEventArgs) Handles HueEmulationSw.Toggled

    End Sub
    '   HOME AUTOMATION SUB GRID CLOSE
    Private Sub HomeAutoCloseBtn_Click(sender As Object, e As RoutedEventArgs) Handles HomeAutoCloseBtn.Click
        AutoGrid.Visibility = Visibility.Collapsed
    End Sub
    Private Sub ScanDevicesBtn_Click(sender As Object, e As RoutedEventArgs) Handles ScanDevicesBtn.Click
        AllHADevices.Items.Clear()
        AllHADevices.Items.Add("Room Lamp")
    End Sub

    '   12
    '   ***********************************************************************
    '   ALL IO setup - I2C SPI UART
    '   ***********************************************************************
    Private Shared gpio As GpioController = Nothing         ' GPIO 
    Public Property I2cPortExpander As I2cDevice            ' PORT EXPANDER - &H20 - MCP23017
    Public Property I2cGyro As I2cDevice                    ' GY-521 MPU6050 &H68
    Public Property I2cCompass As I2cDevice                 ' GY-271 COMPASS
    Public Property I2cMicroChip As I2cDevice               ' support chip for LYNX serial GPS - still deciding! 
    Public Property I2cAltimeter As I2cDevice               ' &H60 - MPL3115-A2 &H60
    Public Property I2cIRTempProde As I2cDevice             ' MLX90614 IR Infrared Body Temperature Sensor
    Public Property IoController As GpioController
    Public Property NrfCEPin As GpioPin
    Public Property MODETECT As GpioPin                     ' PIR Motion Detector
    Public Property Nrf As SpiDevice
    Private Const MoPin As Integer = 4                      ' GPIO4 PIR sensor
    Private Const I2C_CONTROLLER_NAME As String = "I2C1"
    Private Const SPI_CONTROLLER_NAME As String = "SPI0"
    Private Const SPI_CHIP_SELECT_LINE As Integer = 0    ' Line 0 maps To physical pin number 24 On the RPi2 Or RPi3 CE0 - GPIO8
    Private Const PORT_EXPANDER_I2C_ADDRESS As Byte = &H20              ' a0-a2 - gnd.(LOW)
    Private Const NRF24_CE_PIN As Integer = 22           '   GPIO22

    ' Initialize the I2C Port expander
    Private Async Sub Init_I2C_Port()
        Try
            Dim i2cSettings = New I2cConnectionSettings(PORT_EXPANDER_I2C_ADDRESS) With {
                .BusSpeed = I2cBusSpeed.FastMode
            }
            Dim deviceSelector As String = I2cDevice.GetDeviceSelector(I2C_CONTROLLER_NAME)
            Dim i2cDeviceControllers = Await DeviceInformation.FindAllAsync(deviceSelector)
            I2cPortExpander = Await I2cDevice.FromIdAsync(i2cDeviceControllers(0).Id, i2cSettings)
        Catch err As Exception
            Debug.WriteLine("Exception: {0}", err.Message)
            Return
        End Try
    End Sub
    ' Initialize the GPIO pin for the NRF24 CE signal
    Private Sub Init_NRF24_CE()
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
    ' Initialize the SPI bus with the nrf24 attached if found.
    Private Async Sub InitNRF24()
        Try
            ' Create SPI initialization settings                               
            ' Datasheet specifies maximum SPI clock frequency of 10MHz         
            Dim settings = New SpiConnectionSettings(SPI_CHIP_SELECT_LINE) With {
                .ClockFrequency = 5000000,
                .Mode = SpiMode.Mode3 ' was mode0
                }
            Dim spiAqs As String = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME)
            ' Find the selector string for the SPI bus controller          
            Dim devicesInfo = Await DeviceInformation.FindAllAsync(spiAqs)
            Nrf = Await SpiDevice.FromIdAsync(devicesInfo(0).Id, settings)
            ' Add chip specific initialization here (NRF2401L)
            ' Read the status register to see if there is a radio attached.
        Catch ex As Exception
            'Throw New Exception("NRF24 initialization failed", ex)
            PassMessage("NRF24", "...initialization failed!")
        End Try

    End Sub
    '   Init MOTION PIN GPIO4
    Private Sub Init_MOTION()
        '   set pin as input
        Try
            IoController = GpioController.GetDefault()
            ' Get the default GPIO controller on the system 
            ' Initialize a pin as input for the Motion Detect line from the PIR Module.  
            MODETECT = IoController.OpenPin(MoPin)
            MODETECT.SetDriveMode(GpioPinDriveMode.Input)
        Catch err As Exception
            Throw New Exception("GPIO initialization failed", err)
        End Try
    End Sub
    '   Signal to UI that motion has been detected
    Private Sub MotionDetected_Alert()
        '   is security on , check switch in admin menu
        '   by default is off
        If PIRSensorMode.IsOn Then
            '   check pin 4 to see if it is HIGH (True)
            If MODETECT.Read = GpioPinValue.High Then
                MotionDetectedInd.Visibility = Visibility.Visible
                Talker(Zira, "Hello")
            Else
                MotionDetectedInd.Visibility = Visibility.Collapsed
            End If
        End If
    End Sub


    '   ***********************************************************
    '   THE SENSORS SUB GRID
    Private Sub SensorsBtn_Click(sender As Object, e As RoutedEventArgs) Handles SensorsBtn.Click
        ToolsTwoGrid.Visibility = Visibility.Collapsed
        SensorsGrid.Visibility = Visibility.Visible
    End Sub

    Private Sub ScanSensorsBtn_Click(sender As Object, e As RoutedEventArgs) Handles ScanSensorsBtn.Click
        GetI2C()

    End Sub
    Private Sub CloseSensorsBtn_Click(sender As Object, e As RoutedEventArgs) Handles CloseSensorsBtn.Click
        SensorsGrid.Visibility = Visibility.Collapsed
    End Sub


    Private Async Sub GetI2C()
        Dim Results As IEnumerable(Of Byte) = Await FindDevicesAsync()
        If Results IsNot Nothing Then
            contentsTxt.Text = Results.ToString
        ElseIf Results Is Nothing Then
            infoTxt.Text = "No Devices found!"
        End If

    End Sub


    Public Shared Async Function FindDevicesAsync() As Task(Of IEnumerable(Of Byte))
        Dim returnValue As IList(Of Byte) = New List(Of Byte)()
        ' *** Get a selector string that will return all I2C controllers on the system 
        Dim aqs As String = I2cDevice.GetDeviceSelector()
        ' *** Find the I2C bus controller device with our selector string 
        Dim dis = Await DeviceInformation.FindAllAsync(aqs).AsTask()
        If dis.Count > 0 Then
            Const minimumAddress As Integer = 8
            Const maximumAddress As Integer = 77
            For address As Byte = minimumAddress To maximumAddress
                Dim settings = New I2cConnectionSettings(address)
                settings.BusSpeed = I2cBusSpeed.FastMode
                settings.SharingMode = I2cSharingMode.[Shared]
                ' *** Create an I2cDevice with our selected bus controller and I2C settings 
                Using device As I2cDevice = Await I2cDevice.FromIdAsync(dis(0).Id, settings)
                    If device IsNot Nothing Then
                        Try
                            Dim REGISTER_ID As Object = Nothing
                            Dim writeBuffer As Byte() = New Byte(0) {REGISTER_ID}
                            device.Write(writeBuffer)
                            ' *** If no exception is thrown, there is a devie at this address. 
                            Dim readBuffer As Byte() = New Byte(0) {}
                            device.Read(readBuffer)
                            If readBuffer(0) = &H2D Then ' locating a specific device
                                returnValue.Add(address)
                            End If
                            returnValue.Add(address)
                            ' *** If the address is invalid, an exception will be thrown. 
                        Catch
                        End Try
                    End If
                End Using
            Next
        End If
        Return returnValue
    End Function

    Private Async Sub InitSpiBus()
        'this is where all the fun is, the spi 0 i/f will manage the NRF24 interface.
        Dim spiSettings = New SpiConnectionSettings(0)
        Dim spiAqs = SpiDevice.GetDeviceSelector("SPI0")
        Dim devicesInfo = Await DeviceInformation.FindAllAsync(spiAqs)
        Dim ADC = Await SpiDevice.FromIdAsync(devicesInfo(0).Id, spiSettings)
    End Sub



    '   UART i/f
    'Public Event CharacterReceived(ByVal sender As CharacterReceivedEventArgs)


    '   13
    '   ******************************************************
    '   Filer Stuff  Media Stuff...

    Dim SelectedPicture As StorageFile
    Dim SelectedSong As StorageFile
    Public queryOptions As QueryOptions
    Dim currentFolder As StorageFolder
    Private mediaFileExtensions As String() = {
        ".qcp", ".wav", ".mp3", ".m4r", ".m4a", ".aac", ".dat",
        ".amr", ".wma", ".3g2", ".3gp", ".mp4", ".wm", ".gif",
        ".asf", ".3gpp", ".3gp2", ".mpa", ".adt", ".adts",
        ".pya", ".wm", ".m4v", ".wmv", ".asf", ".mov", ".jpeg",
        ".mp4", ".3g2", ".3gp", ".mp4v", ".avi", ".pyv", ".tiff",
        ".3gpp", ".3gp2", ".bmp", ".png", ".jpg", ".txt", ".json"}

    Private Sub SetQueryOptions()
        queryOptions = New QueryOptions(CommonFileQuery.OrderByName, mediaFileExtensions) With {
            .FolderDepth = FolderDepth.Shallow
        }
    End Sub
    '   14
    '   ***********************************************************************
    '   Network and Resident Login Stuff
    Private resourceName As String = "My App"
    Private defaultUserName As String
    Private OOBENetworkPageDispatcher As CoreDispatcher
    Private Automatic As Boolean = True
    Private ResidentPass As PasswordCredential = Nothing
    Public ReadOnly Property NetworkReport As WiFiNetworkReport
    Dim networks As IList(Of WiFiAvailableNetwork)
    Public ReadOnly Property Timestamp As DateTimeOffset
    Public Event NetworkProfileChanged(ByVal sender As NetworkProfileChangedEventHandler)

    '   ALL APP PASSWORD BOX STUFF
    Private Sub CancelPassBtn_Click(sender As Object, e As RoutedEventArgs) Handles CancelPassBtn.Click
        PasswordGrid.Visibility = Visibility.Collapsed
    End Sub
    Private Sub OKPassBtn_Click(sender As Object, e As RoutedEventArgs) Handles OKPassBtn.Click
        PasswordGrid.Visibility = Visibility.Collapsed
        CheckPassword()
    End Sub

    Private Sub CheckPassword()
        If UserPassword.Password = "slasher" Then
            PassMessage("Login: ", "OK  -  Welcome Admin!")
        End If
    End Sub

    Private Sub LoginBtn_Click(sender As Object, e As RoutedEventArgs) Handles LoginBtn.Click
        AdminGrid.Visibility = Visibility.Collapsed
        PasswordGrid.Visibility = Visibility.Visible
        UserPassword.Password = ""
    End Sub

    '   RESIDENT LOGIN
    Private Sub NetworkLoginBtn_Click(sender As Object, e As RoutedEventArgs) Handles NetworkLoginBtn.Click
        ResidentLogin()
    End Sub

    Private Sub Login()
        Dim loginCredential = GetCredentialFromLocker()
        If loginCredential IsNot Nothing Then
            loginCredential.RetrievePassword()
        Else
            loginCredential = GetLoginCredentialUI()
        End If

        ServerLogin(loginCredential.UserName, loginCredential.Password)
    End Sub

    Private ReadOnly Property GetLoginCredentialUI As PasswordCredential
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Private Sub ServerLogin(userName As String, password As String)
        Throw New NotImplementedException()
    End Sub

    Private Function GetCredentialFromLocker() As PasswordCredential
        Dim credential As PasswordCredential = Nothing
        Dim vault = New PasswordVault()
        Dim credentialList = vault.FindAllByResource(resourceName)
        If credentialList.Count > 0 Then
            If credentialList.Count = 1 Then
                credential = credentialList(0)
            Else
                defaultUserName = GetDefaultUserNameUI()
                credential = vault.Retrieve(resourceName, defaultUserName)
            End If
        End If
        Return credential
    End Function

    Private ReadOnly Property GetDefaultUserNameUI As String
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Private Sub ResidentLogin()
        Dim resource As String = Nothing
        'check to see if a vault exists then use current vault, else create a new one
        Dim vault = New PasswordVault()
        vault.Add(New PasswordCredential(resource, NetworkUser.Text.ToString, NetworkPassword.Password.ToString))

        'NetworkLoginGrid.Visibility = Visibility.Collapsed
    End Sub

    Private Sub NetworkLoginCancelBtn_Click(sender As Object, e As RoutedEventArgs) Handles NetworkLoginCancelBtn.Click

        NetworkLoginGrid.Visibility = Visibility.Collapsed
    End Sub



    '   15
    '   ******************************************************************************
    '   GEO LOCATION
    Public Event OnLocationChanged(ByVal sender As OnLocationChangedEventHandler)
    '   update current location
    Private Sub UpdateLocation(sender As OnLocationChangedEventHandler)
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

    '   ***********************************************************************



    '   16
    '   ***********************************************************************
    '   All Radios

    Public ReadOnly Property KindofRadio As RadioKind

    '   17
    '   ***********************************************************************
    '   System stuff
    Private systemPresenter As SystemProperties
    Public MyAlarmPoints As TimeSpan()

    Public Enum MYSYSTEMDEVICES
        GYRO
        COMPASS
        GPS
        NRF24
        MOTION
        FINGERPRINT
        CAMERA
        SOUNDCARD
    End Enum

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

    '   Create a few system defaults
    Public Property XMax As Integer = 800
    Public Property YMax As Integer = 480
    Public Property MyRotation As Integer = 0
    Public Property MyXPos As Integer
    Public Property MyYPos As Integer
    Public Property MyZPos As Integer
    Public Property PERSON_UNKNOWN As Boolean = False

    '   18
    '   CLOCKS AND TIMERS   ***************************************************
    '
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
        'Start a 2 millisec count
        Dim two_milli_tmr As DispatcherTimer = New DispatcherTimer()
        AddHandler two_milli_tmr.Tick, AddressOf TwoMilliSecTimer_Tick
        two_milli_tmr.Interval = TimeSpan.FromMilliseconds(2)
        two_milli_tmr.Start()
    End Sub
    '   timers  two millisec
    Private Sub TwoMilliSecTimer_Tick(sender As Object, e As Object)
        UpdateDragonfly()
    End Sub
    '   timers  100 millisecond count
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
    '   timers  1 SEC for obvious reasons...
    Private Sub OneSecTimer_Tick(ByVal sender As DispatcherTimer, args As Object)
        Check_Message()
        ONE_SEC = ONE_SEC + 1
        MotionDetected_Alert()
        CheckClock()
    End Sub
    '   Check to see if the clock is visible
    Private Sub CheckClock()
        If ClockGrid.Visibility = Visibility.Visible Then
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
        End If
    End Sub
    '   19
    '   ***********************************************************************
    '   GRIDS STUFF - RESTORE MAIN
    Private Sub RestoreMain()
        HideAllGrids()
        PowerBtn.Visibility = Visibility.Visible
        SystemInfoDisplayBtn.Visibility = Visibility.Visible
        ToolsDisplayBtn.Visibility = Visibility.Visible
    End Sub
    '   HIDE ALL GRIDS...
    Private Sub HideAllGrids()
        ErrorsGrid.Visibility = Visibility.Collapsed
        RobotMotorGrid.Visibility = Visibility.Collapsed
        RobotGrid.Visibility = Visibility.Collapsed
        VisionGrid.Visibility = Visibility.Collapsed
        PictureGrid.Visibility = Visibility.Collapsed
        ClockGrid.Visibility = Visibility.Collapsed
        DateGrid.Visibility = Visibility.Collapsed
        CalendarGrid.Visibility = Visibility.Collapsed
        PowerOptionsGrid.Visibility = Visibility.Collapsed
        PowerBtn.Visibility = Visibility.Collapsed
        'StatusGrid.Visibility = Visibility.Collapsed
        SettingsGrid.Visibility = Visibility.Collapsed
        WifiConnectGrid.Visibility = Visibility.Collapsed
        BluetoothActivityGrid.Visibility = Visibility.Collapsed
        AudioManagerGrid.Visibility = Visibility.Collapsed
        SystemInfoGrid.Visibility = Visibility.Collapsed
        ToolsMenuGrid.Visibility = Visibility.Collapsed
        FileManagerGrid.Visibility = Visibility.Collapsed
        LocationOrientationGrid.Visibility = Visibility.Collapsed
        BrowserGrid.Visibility = Visibility.Collapsed
        BrowserMenuGrid.Visibility = Visibility.Collapsed
        AutoGrid.Visibility = Visibility.Collapsed
        AdminGrid.Visibility = Visibility.Collapsed
        InstalledAppsGrid.Visibility = Visibility.Collapsed
        IATitleGrid.Visibility = Visibility.Collapsed
        WeatherGrid.Visibility = Visibility.Collapsed
        TimePickerGrid.Visibility = Visibility.Collapsed
        ToolsTwoGrid.Visibility = Visibility.Collapsed
        ConfigureGrid.Visibility = Visibility.Collapsed
        GuestMgmtGrid.Visibility = Visibility.Collapsed
        SpeechPlusGrid.Visibility = Visibility.Collapsed
        PasswordGrid.Visibility = Visibility.Collapsed
        NetworkLoginGrid.Visibility = Visibility.Collapsed
        SensorsGrid.Visibility = Visibility.Collapsed
        GetUserTextDialogueGrid.Visibility = Visibility.Collapsed
        FingerprintGrid.Visibility = Visibility.Collapsed
    End Sub

    '   20
    '   ***********************************************************************
    '   VOICE STUFF
    '   Speech Recognition and Synthesis and Audio

    Dim MyListener As New SpeechRecognizer
    Dim MySpeaker As New SpeechSynthesizer
    Dim MySpeechStream As SpeechSynthesisStream
    Public MyBackgroundAudioStream As Stream

    Private isPlaying As Boolean = False
    Private isListening As Boolean = False
    Private isTalking As Boolean = False
    Private voice As VoiceInformation
    Private Const SRGS_FILE As String = "Grammar/grammar.xml"   ' Grammar File
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

    'Private Async Sub SMLTestBtn_Click(ByVal sender As Object, ByVal args As RoutedEventArgs) Handles SMLTestBtn.Click
    'Try
    '  Dim synthStream As SpeechSynthesisStream = Await MySpeaker.SynthesizeSsmlToStreamAsync(Jabberwoky)
    '        MediaElement.SetSource(synthStream, synthStream.ContentType)
    ' Catch ex As Exception
    ' 'StatusTxt.Text = ex.ToString
    ' End Try
    'End Sub

    Private Async Sub Start_Listening()
        Talker(Mark, "I have started listening.")
        Dim result As SpeechRecognitionResult = Await MyListener.RecognizeAsync()
    End Sub
    Private Async Sub Stop_Listening()
        Await MyListener.StopRecognitionAsync()
        Await MyListener.ContinuousRecognitionSession.StopAsync()
        Talker(Mark, "I have stopped listening.")
    End Sub
    Private Sub Set_Voice(name)
        Dim voice = SpeechSynthesizer.AllVoices.Single(Function(v) v.DisplayName.Contains(name))
        MySpeaker.Voice = voice
    End Sub
    Private Async Sub Talker(voice, message)
        If Not MuteSpeechToggle.IsOn Then
            Try
                If (Not isTalking) Then
                    isTalking = True
                    MySpeaker.Voice = SpeechSynthesizer.AllVoices.FirstOrDefault(Function(v) v.DisplayName.Contains(voice))
                    MySpeechStream = Await MySpeaker.SynthesizeTextToStreamAsync(message)
                    MyPlayer.SetSource(MySpeechStream, MySpeechStream.ContentType)
                    isTalking = False
                End If
            Catch ex As Exception
                PassMessage("Talker fail!", ex.ToString)
            End Try
        End If
    End Sub
    '   Close SpeechPlus Grid
    Private Sub CloseSpeechPlusBtn_Click(sender As Object, e As RoutedEventArgs) Handles CloseSpeechPlusBtn.Click
        SpeechPlusGrid.Visibility = Visibility.Collapsed
        RestoreMain()
    End Sub
    '   listen on in Speech Plus grid
    Private Sub ListenEnableSw_Toggled(sender As Object, e As RoutedEventArgs) Handles ListenEnableSw.Toggled
        If ListenEnableSw.IsOn Then
            Stop_Listening()
        Else
            Start_Listening()
        End If
    End Sub
    '   mute speech
    Private Sub MuteSpeechToggle_Toggled(sender As Object, e As RoutedEventArgs) Handles MuteSpeechToggle.Toggled
        If MuteSpeechToggle.IsOn Then
            ContinueSpeaking()
        ElseIf Not MuteSpeechToggle.IsOn Then
            StopTalking()
        End If
    End Sub

    Private Sub ContinueSpeaking()
        isTalking = False
    End Sub

    Private Sub StopTalking()
        isTalking = True
    End Sub

    '   21
    '   AUDIO PLAYER
    '   ************************************************************************
    '   Music populate
    Private Async Sub ShowAllMusic()
        currentFolder = Nothing
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
    Private Async Sub MusicFiles_Tapped(ByVal sender As Object, ByVal args As TappedRoutedEventArgs) Handles MusicFilesList.Tapped
        If MusicFilesList.SelectedItem IsNot Nothing Then
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
                SelectedMusicTxt.Text = SelectedSong.DisplayName
                MyBackgroundAudioStream = Await SelectedSong.OpenAsync(accessMode:=FileAccessMode.Read)
                MyPlayer.SetSource(stream:=MyBackgroundAudioStream, mimeType:=SelectedSong.ContentType)
                MyPlayer.Play()
            End If
        Catch ex As Exception
            PassMessage("", ex.Message.ToString)
        End Try
    End Sub
    '   music controls
    Private Sub PauseMusic()
        MyPlayer.Pause()
    End Sub
    Private Sub ResumeMusic()
        MyPlayer.SetSource(MyBackgroundAudioStream, SelectedSong.ContentType)
    End Sub
    '   Audio volume
    Private Sub MainVolumeSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles MainVolumeSlider.ValueChanged
        MyPlayer.Volume = MainVolumeSlider.Value
    End Sub
    '   close self (audio manager)
    Private Sub AudioManagerOKBtn_Click(sender As Object, e As RoutedEventArgs) Handles AudioManagerOKBtn.Click
        AudioManagerGrid.Visibility = Visibility.Collapsed
    End Sub







    '   22
    '   ********************************************************************************************
    '   COMPASS STUFF

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
    Public Sub SetCompassOperatingMode(operatingMode As CompassOperatingMode)
        Try
            ' convention is to specify the register first, and then the value to write to it
            Dim writeBuffer = New Byte(1) {COMPASS_MODE_ADDRESS, (operatingMode)}
            I2cCompass.Write(writeBuffer)
        Catch err As Exception
            Debug.WriteLine("Exception: {0}", err.Message)
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

    '   23
    '   ***************************************************************************************
    '   BROWSER STUFF

    Public MyBookmarks(10, 10) As String
    Private GoogleHome As String = "http://www.google.ca"
    Private BingHome As String = "https://www.bing.com/#"

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

    '   Close browser
    Private Sub BrowserCloseBtn_Click(sender As Object, e As RoutedEventArgs) Handles BrowserCloseBtn.Click
        HideAllGrids()
        MyBrowser.Stop()
        RestoreMain()
    End Sub
    '   bring up the menu sub-grid for the browser
    Private Sub BrowserMenuBtn_Click(sender As Object, e As RoutedEventArgs) Handles BrowserMenuBtn.Click
        Select Case BrowserMenuGrid.Visibility
            Case Visibility.Visible
                BrowserMenuGrid.Visibility = Visibility.Collapsed
                Exit Select
            Case Visibility.Collapsed
                BrowserMenuGrid.Visibility = Visibility.Visible
        End Select
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
    '   Refesh  browser page
    Private Sub ReloadBrowserPage_Click(sender As Object, e As RoutedEventArgs) Handles ReloadBrowserPage.Click
        MyBrowser.Refresh()
    End Sub
    '   cancel browser page load
    Private Sub CancelBrowserPageLoadBtn_Click(sender As Object, e As RoutedEventArgs) Handles CancelBrowserPageLoadBtn.Click
        MyBrowser.Stop()
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
            PassMessage("Wallpaper", "No files found!")
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

    Private Sub CloseConfigBtn_Click(sender As Object, e As RoutedEventArgs) Handles CloseConfigBtn.Click
        ConfigureGrid.Visibility = Visibility.Collapsed
    End Sub





    '   24
    '   ***********************************************
    '   GUEST MGMT SUB COMMANDS
    Private Sub CloseGuestMenuBtn_Click(sender As Object, e As RoutedEventArgs) Handles CloseGuestMenuBtn.Click
        GuestMgmtGrid.Visibility = Visibility.Collapsed
    End Sub

    Private Sub AddNewGuestBtn_Click(sender As Object, e As RoutedEventArgs) Handles AddNewGuestBtn.Click

    End Sub

    Private Sub RemoveGuestBtn_Click(sender As Object, e As RoutedEventArgs) Handles RemoveGuestBtn.Click

    End Sub

    '   25
    '   ***************************************************
    '   admin sub-grid stuff
    Private Sub AdminOKBtn_Click(sender As Object, ByVal args As RoutedEventArgs) Handles AdminOKBtn.Click
        RestoreMain()
    End Sub



End Class

