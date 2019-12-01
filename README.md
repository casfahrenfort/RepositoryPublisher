# RepositoryPublisher
Prototype made for University of Amsterdam Software Engineering Master Thesis.

[Check out the live demo, hosted on Microsoft Azure.](https://repopublisher.z6.web.core.windows.net/)

This tool allows you to publish version control repositories to persistent publishing systems. The tool is built using an Angular front end and ASP .NET Core back end. An additional MongoDB database is connected to.

To build and execute the prototype:

* Install Angular CLI
* Run ```ng serve --open``` in ```FrontEnd```
* Install Visual Studio and ASP .NET Core
* Open and run the solution in ```BackEnd/ThesisPrototype```

#### Supported Version Control systems
1. Git
2. Subversion (SVN)

#### Supported publishing systems
1. [EUDAT - B2SHARE training environment](https://trng-b2share.eudat.eu/)
2. [figshare](https://figshare.com/)
3. [Harvard Dataverse demo enviornment](https://demo.dataverse.org/)
