# NetworkVisualizer
NetworkVisualizer is a project co-developed by SDProductions and Aidan Smith. The end goal is to create a fluid, functional, multi-platform and scalable website which can be accessed and will show aggregated statistics and data about a target network. Currently, a site showing the latest commit or the next version can be found [here](https://networkvisualizer.azurewebsites.net/).

The premises of operation is simple, a device which has two network interfaces, one set to monitoring mode, is deployed and connected to the target network. This device runs the [packet sniffer](https://github.com/AllDoge/Network-Analyzer-Backend) and sends the data via HttpPOST to this website. The data gets added to the database, and it gets displayed on the front page graphs. If you would like more information on how we plan to use the data, you can visit our [about](https://networkvisualizer.azurewebsites.net/About) and [terms of service](https://networkvisualizer.azurewebsites.net/Terms) pages.

It is important a few key features are missing right now, and that we don't recommend taking this site and running it for yourself just yet. A fluid and complete administrator panel is being implemented currently, but is no where near done and as such knowledge of the app structure is required to modify app behaviour. Code is still unoptimized and messy, although optimizations are being worked on. A full, complete user experience is also definitely not complete, although being worked on. The site is functional, but not pretty. Smaller details like more graphs are also not present at this stage.

# Forking and cloning this repo

When forking or cloning this repo, ensure you have performed the following steps or the site may not function correctly.
1) Run Update-Database in the NuGet Package Manager Console - since your machine likely does not have the SQL database already there
2) In the User database table, create a new user account and password like root and root.
3) Take a look at connection strings and passwords found in the config files, and decide if you need to change them
4) We highly recommend you change passwords as having it remain the default could subject yourself to unwanted posts (rare, but maybe)

# Deploying this to production

A few files required for deploying to production have been omitted from this repo for reasons of not leaking my passwords. You need the following to deploy this site for your own use in production:
1) appsettings.production.json - this file needs the SqlConnection string for your database that you are using
2) Steps 2 and 3 from forking and cloning this repo

# Other notes

Yo, we ain't liable for what you do with this site. Be responsible, get permission if you are slapping this somewhere in public.
