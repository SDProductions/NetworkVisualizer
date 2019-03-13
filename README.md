# NetworkVisualizer
Takes post requests from a packet sniffer and compiles data into pretty graphs to show where network traffic is going.
An unfinished version of the site (likely the same version as the latest commit here) can be found at https://networkvisualizer.azurewebsites.net.

# Input
HttpPOST Request (string password, string json) where password is predetermined and synced with both this and the sniffer, and json is of format List<Packet> (struct found in models), and Id is optional. ID value in Packet structure is optional as Entity Framework fills that in automatically.

# Output
Stacked area chart showing top 4 domains and an 'other sites' category that displays network data from the previous 24 hours.

# Magic black box materials
ASP.NET MVC Core (.NET Core 2.1) with Entity Framework Core & SQL for database.
