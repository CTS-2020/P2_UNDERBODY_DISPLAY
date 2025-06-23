20250623

previous note:
cannot move to other screen since have a checking that if the curretn window is not in the designated screen whole app wil be restarted to make that the screens is on the right screens.

changes 
metalchipinspection
1. remove the cert checking
right click on the solution adn click properties, navigate to the signing tab and untick the "Sign the ClickOne manifest"

2. update the assembly name and .exe file icon to "MetalChipInspection" and perodua_ico

3. add the try catch block at the program.cs

error happen will be log into a file named crashlog.txt and stored and the same level of the .exe file in a folder
error that can catch and its repective iisue:

1. A critical error occurred during application startup. Check crashlog.txt for details.
crashlog.txt content:
2025-06-23 14:19:50 [Main Method Exception]:
System.IO.FileNotFoundException: The configuration file 'appsettings.json' was not found and is not optional. The expected physical path was 'C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\bin\Debug\appsettings.json'.
   at Microsoft.Extensions.Configuration.FileConfigurationProvider.HandleException(ExceptionDispatchInfo info)
   at Microsoft.Extensions.Configuration.FileConfigurationProvider.Load(Boolean reload)
   at Microsoft.Extensions.Configuration.FileConfigurationProvider.Load()
   at Microsoft.Extensions.Configuration.ConfigurationRoot..ctor(IList`1 providers)
   at Microsoft.Extensions.Configuration.ConfigurationBuilder.Build()
   at WindowsFormsApp1.Program.Main() in C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Program.cs:line 39
   
reason:
appsetting file is not found.

solution:
make sure the appsetting file is at the same level with the .exe file in the folder,

2. 
crashlog.txt content;
2025-06-23 14:25:00 [Form2 Thread Exception]:
System.IO.FileNotFoundException: C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\bin\Debug\Resources\3_R.png
   at System.Drawing.Image.FromFile(String filename, Boolean useEmbeddedColorManagement)
   at System.Drawing.Image.FromFile(String filename)
   at WindowsFormsApp1.Form2.UpdateImage() in C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Form2.cs:line 454
   at WindowsFormsApp1.Form2.GetData() in C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Form2.cs:line 207
   at WindowsFormsApp1.Form2.InitializePart() in C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Form2.cs:line 367
   at WindowsFormsApp1.Form2..ctor(PassIn passin) in C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Form2.cs:line 45
   at WindowsFormsApp1.Program.<>c__DisplayClass1_0.<Main>b__3() in C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Program.cs:line 128
   
reason:
resource folder is not found

solution:
make sure the resource folder is at the same level with the .exe file in the folder,


3. A critical error occurred during application startup. Check crashlog.txt for details.

crashlog.txt content
2025-06-23 14:32:23 [Main Method Exception]:
System.InvalidOperationException: Invalid screen index 4 for Log_Screen. Available screens: 2.
   at WindowsFormsApp1.Program.Main() in C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Program.cs:line 97

reason:
the screen index is out of range for the available screen listing.

solution:
update the screen index in the appsetting to be within the range.

4. A critical error occurred during application startup. Check crashlog.txt for details.

crashlog.txt content
2025-06-23 14:37:50 [Main Method Exception]:
System.InvalidOperationException: Failed to load padding values for image type D27A. ---> System.InvalidOperationException: Configuration value for padding (Padding:RightImage:D27A:RightPadding) is missing or empty.
   at WindowsFormsApp1.Program.GetAndParsePaddingValue(IConfiguration configuration, String section, String imageType, String paddingType) in C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Program.cs:line 227
   at WindowsFormsApp1.Program.GetPaddingValues(IConfiguration configuration, String imageType) in C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Program.cs:line 207
   --- End of inner exception stack trace ---
   at WindowsFormsApp1.Program.GetPaddingValues(IConfiguration configuration, String imageType) in C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Program.cs:line 217
   at WindowsFormsApp1.Program.Main() in C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Program.cs:line 59

reason:
the padding value set in the appsetting is empty or missing.

solution:
update to a valid padding value of missed part in the appsetting file.

5. and a exception for single screen:
An error occurred while running forms. Check crashlog.txt for details.
