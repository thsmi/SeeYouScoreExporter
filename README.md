# SeeYouScoreExporter

SeeYou Competition recently got all new CUBX files and other major changes. Beside lots of improvements, sadly the xml files with the scooring are gone.   

Which means displaying the results on a projector via SeeYou's webview tool or in a smartphone is no more possible. Using the SoaringSpot Website is for both usecases not an option.  

This tool brings the XML/HTML files as well as the webview tool back. It loads the results from soaringspot via the public api and renders them based on generic templates into files. In case the generated files are HTML they can be directly displayed in a slideshow. The templates are geneirc, which allows to generate almost any text based file like json, html or xml.


# Prerequisites 
1. Create a API Key for your Competition
As first step you need to create an api key for your competition.
Refer to http://support.naviter.com/support/solutions/articles/5000632190-public-api-for-soaring-spot

2. Download the binary.
It is a command line tool. You need at least a .NET 4 Framework on our Computer.

3. .NET Framwork
Make sure the .NET Framework 4.0 or newer is installed on your Computer. 

# Configure the tool. 

It is a command line tool which needs to be configured before use. The confguration is interactive. This means with each call you can change only one setting at a time. In case you ommit the setting's parameters the current status will be displayed.

The settings are protable and stored in plain text in the ''SeeYouSoreExporter.config'' next to the ''SeeYouScoreExporter.exe''. 

Both the Export (Extracting and rendering the results) and Slideshow (Presenting the HTML result files in a slideshow) are separate module they have a separate configuration interface and can be run independently from eachother.

## Configuring the HTML/XML/JSON Export

In order to generate result files, you need to configure at least the api keys, the competition id and the template.

### Api keys/Credentials

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

### Contest Id

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

### Templates

Template files are used to render the results. They are plaintext files with tokens/placeholders which will be replaced while parsing. The project includes examples for such templates in the tpl folder.

In this example we want to create the daily score which is based upon the template named daily.html.tpl. 
So the template name will be like the filename without the tpl extension in our case "daily.html". The generated file's names have always the following pattern YYYY-MM-DD_CLASS_TEMPLATE e.g. 2016-04-30_15_meter_daily.html.

You can specify as many templates as you want. Each templates is run against all classes and tasks.

By default the templates are expected to be in the same directory as the SeeYouScoreExporter. The same applies to the generated files. In case you want a cleaner structure, use the "--export directories" command to specify the the default locations.

````
SeeYouScoreExporter --export templates daily.html
````

### Runing

Now as everything is configured run the programm:

````
SeeYouScoreExporter --run
````

The programm runs in an endless loop. You can stop by pressing the Control and the C key.

The default update interval is set to 2 minutes. You may ajust it with the "--export refresh" command.

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
