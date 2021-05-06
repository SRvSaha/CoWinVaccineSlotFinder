# CoWinVaccineSlotFinder
Console App to Fetch the Available Slots & Booking the Appointment Schedule for COVID-19 Vaccination using the Publically available [APISetu APIs](https://apisetu.gov.in/public/marketplace/api/cowin/cowin-public-v2#/) from Govt Of India. 


## Technical Details

It's a simple Cross-Platform Console Application being developed using .NET Core 3.1 and C#.

Currently, only the [calenderByDistrict API](https://apisetu.gov.in/public/marketplace/api/cowin/cowin-public-v2#/Appointment%20Availability%20APIs/calendarByDistrict) is integrated to get all the available slots in a particular district and to book the slot in First-Come-First-Serve Basic, the
 [appointmentSchedule API](https://apisetu.gov.in/public/marketplace/api/cowin/cowin-protected-v2#/Vaccination%20Appointment%20APIs/schedule) is used.
 
We have both Public APIs and Protected APIs exposed, but a general observation was the Public APIs return stale data and since the slots are gone literally in seconds, so I had to use the Protected APIs.

Well, what's probably going on in your mind is, how do I get the Authentication Information to call the Protected APIs?

The answer lies in how well you know to use the F12 Developer Tools of your brower.

You'll be able to get the Bearer Token (Authentication Token) after you have signed in using your Mobile Number and OTP, in the Sessions Tab or in the Response of  OTP Verification Request you are making. 

Make use of the Network Tab inside F12 Window wisely, and you are good to go!

Settings are configurable and can be modified from appsettings.json

You'll need to grab your beneficiaryId, Auth Token and that's it. Rest of the stuff are self-explanatory.

## How to Use:

### For Developers or Curious Minds:

If you have Visual Studio installed, go ahead an Clone the Repository, Open the SLN file, Ctrl + F5 and Boom!

Well, not interesting right?

Yeah, basically you've this Project Named COWIN which has appsettings.json which does most of the magic.
Rest are there inside the Models directory.

Clean Coding Practices has been followed during the development of the Application within a span of 2 days apart from Office Hours. So, you won't find proper Exception Handling, using Dependency Injection or Logging or even Documenentation, duh! 

I know, I know, it's not cool, but see it's all about quick Time to Market First and then doing one thing at a time. 

I'll be more than happy to have PRs with modifications.

### For Folks who just want to get shit done

Go to the Releases Section of the Application, download the ZIP file, exact it, Modify the Setting inside appsettings.json ,Run the Executable (EXE in case of Windows) . Enjoy!

If you'd like to do it the hard way, clone it, build it and run it.

NB: appsettings.json play the major role for accessing and booking and filtration of searches. Fiddle with it! Appologies that the Code doesn't have inline documentation, but code is readable and self explanatory. In case of any issue, feel free to raise an Issue.

As simple as that!

Cheers!
