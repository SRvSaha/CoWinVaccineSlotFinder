# CoWinVaccineSlotFinder
Console App to Fetch the Available Slots & Booking the Appointment using the Publically available [APISetu APIs](https://apisetu.gov.in/public/marketplace/api/cowin/cowin-public-v2#/) from Govt Of India. 


## Technical Details

It's a simple Console Application being developed using .NET Core 3.1 and C# and 
Currently, only calenderByDistrict API (https://apisetu.gov.in/public/marketplace/api/cowin/cowin-public-v2#/Appointment%20Availability%20APIs/calendarByDistrict) is integrated to get all the available slots in a particular district.

Settings are configurable and can be modified from appsettings.json

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

Go to the Releases Section of the Application, download the Package, Run it. Enjoy!

If you'd like to do it the hard way, clone it, build it and run it.

NB: appsettings.json play the major role for accessing and booking and filtration of searches. Fiddle with it! Appologies that the Code doesn't have inline documentation, but code is readable and self explanatory. In case of any issue, feel free to raise an Issue.

As simple as that!

Cheers!
