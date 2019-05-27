# NetworkVisualizer
[![Build Status](https://travis-ci.com/SDProductions/NetworkVisualizer.svg?branch=master)](https://travis-ci.com/SDProductions/NetworkVisualizer)

NetworkVisualizer is a project co-developed by SDProductions and Aidan Smith. The end goal is to create a fluid, functional, multi-platform and scalable website which can be accessed and will show aggregated statistics and data about a target network. A live demo of the site used to exist, however, due to the requirement of some kind of SQL database for it to function and the sad reality that those cost money, the site is no longer up. Instead, visit [here](http://wisdomduck.azurewebsites.net) for some quality wisdom.

The premises of operation is simple, a device which has two network interfaces, one set to monitoring mode, is deployed and connected to the target network. This device runs the [packet sniffer](https://github.com/AllDoge/Network-Analyzer-Backend) and sends the data via HttpPOST to this website. The data gets added to the database, and it gets displayed on the front page graphs. If you would like more information on how we plan to use the data, you can read our about page below.

It is important to note a few features regarding user experience are missing right now, and that we don't recommend taking this site and running it for yourself just yet unless you wanted to modify it. A fluid and complete administrator panel is being implemented currently, but is no where near done and as such knowledge of the app structure is required to modify app behaviour. Code is still mostly unoptimized, although optimizations are being worked on. A full, smooth user experience is also not complete, although being actively worked on. The site is functional, but not necessarily pretty.

This won 3rd place at the Mountain View High School STEM Fair! Pretty cool.

# Forking and cloning this repo

When forking or cloning this repo, ensure you have performed the following steps or the site may not function correctly.
1) Run Update-Database in the NuGet Package Manager Console - or, set up a database and change the SQL connection string
2) In Program.cs, change the default config file that gets generated to your needs
3) Change the appsettings.json file to contain your Azure Active Directory subscription info
4) We highly recommend you change passwords as having it remain the default could subject yourself to unwanted posts (rare, but maybe)

# Deploying this to production

A few files required for deploying to production have been omitted from this repo for reasons of not leaking my passwords. Yes, I am aware of things like Azure Key Vaults and the Visual Studio secrets manager and I will look into those later. For now, you need the following to deploy this site for your own use in production:
1) appsettings.production.json - this file needs the SqlConnection string for your database that you are using as well as the Azure AD information found in the development version of this file
2) Steps 2 and 4 from "Forking and cloning this repo"

# About

NetworkVisualizer is developed and maintained by [SDProductions](https://sdproductions.github.io).

NetworkVisualizer is a stable, secure, and scalable website built on the principle idea of cloud technology, headless operation, and managable infrastructure. The site operates using ASP.NET Core 2.1 MVC with an SQL database for information handling, all of which is hosted on Microsoft Azure. We ourselves do none of the packet sniffing - this is done on a separate utility on the target network. This site receives information through HttpPOST requests and has a layer of authentication to prevent unauthorized access. Our site automatically deletes information after 24 hours of recieving it, however, we state that information can remain on our servers for up to 30 days before deletion in case we would like to add increased analytical functionality. For more information on this, see our Terms of Service.

# Legal
Certain assets and functions of this website are made by third parties not affiliated, in contact, or related to SDProductions. Google's charting scripts, found [here](https://developers.google.com/chart/interactive/docs/). A big shoutout goes to [Zoran Maksimovic](https://github.com/zoranmax), creator of [Google.DataTable.Net.Wrapper](https://github.com/zoranmax/GoogleDataTableLib), which is a library that facilitates the creation of JSON objects used by Google's charting script. His library has saved great amounts of time and work. Google.DataTable.Net.Wrapper is licensed under the [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0.html) license. Certain code samples used from Google are also licensed under Apache-2.0. The Google charting scripts are licensed under the [Creative Commons 3.0 Attribution](https://creativecommons.org/licenses/by/3.0/) license. All works belong and are copyrighted by their respective owners.

# Other notes

Yo, we ain't liable for what you do with this site. Be responsible, get permission if you are slapping this somewhere in public.
