# SeeYouScoreExporter

SeeYou Competition recently got all new CUBX files and other major changes. Beside lots of improvements, sadly the xml files with the scooring are gone.   

Which means displaying the results on a projector via SeeYou's webview tool or in a smartphone is no more possible. Using the SoaringSpot Website is for both usecases not an option.  

This tool brings the XML/HTML files as well as the webview tool back. It loads the results from soaringspot via the public api and renders them based on generic templates into files. In case the generated files are HTML they can be directly displayed in a slideshow. If the pages are to long they are automatically scrolled. The templates are generic, which allows to generate almost any text based file like json, html or xml.

# Prerequisites 
* Create a API Key for your Competition

    A description how to get an api key can be found here http://support.naviter.com/support/solutions/articles/5000632190-public-api-for-soaring-spot

* Download the binary
    You get it here https://github.com/thsmi/SeeYouScoreExporter/releases

    It is a command line tool. You need at least a .NET 4 Framework on our Computer.

* .NET Framwork

    Make sure the .NET Framework 4.0 or newer is installed on your Computer. 

# Configuration 

It is a command line tool which needs to be configured before use. The confguration is interactive. This means with each call you can change only one setting at a time. In case you ommit the setting's parameters the current status will be displayed.

The settings are protable and stored in plain text in the ''SeeYouSoreExporter.config'' next to the ''SeeYouScoreExporter.exe''. 

Both the Export (Extracting and rendering the results) and Slideshow (Presenting the HTML result files in a slideshow) are separate module they have a separate configuration interface and can be run independently from eachother.

## HTML/XML/JSON Export Configuration

In order to generate result files, you need to configure at least the api keys, the competition id and the template.

#### Api keys/Credentials

The api keys are used for authentication. Without them you won't be allowed to access any data on soaring spot.

The api keys is are log string they are set by the following call. 
Enclose the keys in double quotes as they may contain special characters which clash with the command line.

```
SeeYouScoreExporter --export credential "YOUR_CLIENT_ID"  "YOUR_SECRET"
```

If you want to verify if the credentials where set correctly. Do the same call but without any parameters.

````
SeeYouScoreExporter --export credentials
````

#### Contest Id

The next step is setting the contest id. The tool exports only one contest. 

If you are unsure about you contest id. You can get a list of all competitions associated with your account by calling:

````
SeeYouScoreExporter --export contest
````

The output should be similar to the following:

````
Contest Id set to
The following contests are associated to your account

  Id...... : 1844
  Name.... : Internationaler HAHNWEIDE Segelflugwettbewerb 2016
  Start... : 2016-04-29
  End .... : 2016-05-07
````

Grab the contest id of your competition, in this example 1844 end set it via

````
SeeYouScoreExporter --export contest 1844
````

#### Templates

Template files are used to render the results. They are plaintext files with tokens/placeholders which will be replaced while parsing. The project includes examples for such templates in the tpl folder.

In this example we want to create the daily score which is based upon the template named daily.html.tpl. 
So the template name will be like the filename without the tpl extension in our case "daily.html". The generated file's names have always the following pattern YYYY-MM-DD_CLASS_TEMPLATE e.g. 2016-04-30_15_meter_daily.html.

You can specify as many templates as you want. Each templates is run against all classes and tasks.

By default the templates are expected to be in the same directory as the SeeYouScoreExporter. The same applies to the generated files. In case you want a cleaner structure, use the "--export directories" command to specify the the default locations.

````
SeeYouScoreExporter --export templates daily.html
````

## Slide show Configuration

The slide show is used to persent html files in fullscreen in an endless loop. The slide show is highdpi aware which means it is designed to work well with 4K Displays.

At minimum you need to specify the template which should be displayed.

#### Templates

You need to specify the template which should be displayed. The files are expeced to have the same naming as with the export module.

If you want to display the exported files from previous example specify daily.html as template.

````
SeeYouScoreExporter --slideshow templates daily.html
````

The lookup path for the slideshow files can be defined by setting ""--slideshow directories". By default the files are expected to be in the same directory as the executable.


# Running

Now as everything is configured run the programm:

````
SeeYouScoreExporter --run
````

The programm runs in an endless loop. You can stop by pressing the Control and the C key.

The full screen slideshow is also closed by "Control+C" or by pressing the "esc" button. 
But unless the main console window is not terminated, the slideshow will respawn automatically.

The export modules default update interval is set to 2 minutes. You may ajust it with the "--export refresh" command.

The slideshow duration can be controlled via "--slideshow duration". 

#### Disabling Modules

Export and slideshow are independent modules. In case you need only the file export you can disable the slideshow module by calling

````
SeeYouScoreExporter --slideshow enabled false
````

or in case you want just the slideshow, you can disable the export module by calling.

````
SeeYouScoreExporter --export enabled false
````

#### Restrict Results 
By default the tool displays and/or genereates files for all available competition days.

If you want to present or generate files just for a specific day use the "day" parameter. Both modules support it. You can either specify a date string "YYYY-MM-DD" or use the strings today and yesterday. In case the day parameter is empty all files for the specified will be displayed or generated.

If you want to restrict both the export or the slideshow to the current day use:

````
SeeYouScoreExporter --export day today
SeeYouScoreExporter --slideshow day today
````

# Troubleshooting

As first step activate the debug mode for the corresponding module.
````
SeeYouScoreExporter --export debug true"
SeeYouScoreExporter --slideshow debug true"
````

It will log exception and their stacktraces as well as failed requests into the files error.log and failedRequest.log.

Setting the credentials is more tricky as it may sound. Even a small type results in a unauthorized response.

The following message on is a strong indicator for bad credentials:
````
Error code: Unauthorized
Failed to load http://api.soaringspot.com/v1/ , 
````

Try to open the failedRequest.log in your browser. It is most likely a HTML file which contains further information 
on the root cause. 

In case you run into exceptions. Create a issue here at github and attach the error.log. It contains the stacktrace which helps to isolate the issue.
