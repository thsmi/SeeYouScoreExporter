# SeeYouScoreExporter

SeeYou Competition recently got all new CUBX files and other major changes. Beside lots of improvements, sadly the xml files with the scooring are gone.   

Which means displaying the results on a projector via SeeYou's webview is no more possible. 
Using the SoaringSpot Website is not very feasible if you want to present the result on a projector.  

This tool brings the XML/HTML Files back. It loads the results from soaringspot via the public api and renders them into files.

The tool uses template file with place holders to render the data. This allows to render the data into almost any 
text based files like json, html or xml.

# Using the tool

1. Create a API Key for your Competition
As first step you need to create an api key for your competition.
Refer to http://support.naviter.com/support/solutions/articles/5000632190-public-api-for-soaring-spot

2. Download the binary.
It is a command line tool. You need at least a .NET 4 Framework on our Computer.

3. Configure the tool. 
Before using the tool you need to configure it.

Changing settings is pseudo interactive. This means you can change on setting per call. In case you ommit the setting's 
parameters the current status will be displayed.

At minimum you need to set the apikey as well as the competition id.

In order to set the api key call. Enclose them in double quotes as they may contain special characters which clash 
with the command line.

```
SeeYouScoreExporter --settings credential "YOUR_CLIENT_ID"  "YOUR_SECRET"
```

If you want to verify if the credentials where set correctly. Do the same call but without any properties.

SeeYouScoreExporter --settings credentials

The next step is setting the competition id. In order to get a list of all competitions associated with your account call

SeeYouScoreExporter --settings contest

The output should look like this:

````
Contest Id set to
The following contests are associated to your account

  Id...... : 1844
  Name.... : Internationaler HAHNWEIDE Segelflugwettbewerb 2016
  Start... : 2016-04-29
  End .... : 2016-05-07
````

Grab the contest id for your competition, in this example 1844 end set it via

````
SeeYouScoreExporter --settings contest 1844
````

Set the templates file(s). The project includes some examples for such templates in the tpl folder. In this example 
we want to create the daily score which is based upon the daily.html.tpl. The template name is the filename without the tpl extension.

You can specify as many templates as you want. Each templates is run all classes and tasks.

By default the templates are expected to be in the same directory as the SeeYouScoreExporter. The same applies to the generated files. 
Use the "--settings directories" command to save the default locations.

````
SeeYouScoreExporter --settings templates daily.html
````

Now as everything is configured run the programm:

````
SeeYouScoreExporter --run
````

The programm runs in an endless loop. You can stop by pressing the Control and the C key.

The default update interval is set to 2 minutes. You may ajust it with the "--settings refresh" command.

== Troubleshooting

As first step activate the debug mode. Call SeeYouScoreExporter --settings debug true".
It will log execption and their stacktraces as well as failed requests into the files error.log and failedRequest.log.

Setting the credentials is more tricky as it may sound. Even a small type results in a not autorized response.

The following message on the command line is a strong indicator for bad credentials:
    Error code: Unauthorized
    Failed to load http://api.soaringspot.com/v1/ , 

Try to open the failedRequest.log in your browser. It is most likely a HTML file which contains further information 
on the root cause. 

In case you run into exceptions. Create a issue here at github and attach the error.log. It contains the stacktrace.
