<%@ page language="C#" validaterequest="false" %>

<script language="C#" runat="server">
public string GetUpTime() {
   
        using (var uptime = new System.Diagnostics.PerformanceCounter("System", "System Up Time")) {
            uptime.NextValue();       //Call this an extra time before reading its value
            TimeSpan ut = TimeSpan.FromSeconds(uptime.NextValue());
            string r=ut.Days+" days, "+ut.Hours+" hours, "+ut.Minutes+" minutes, "+ut.Seconds+" seconds";
            return r;
        }
}

public static string GetDriveInfo()
{
       string r = "";
       System.IO.DriveInfo[] allDrives = System.IO.DriveInfo.GetDrives();

        foreach (System.IO.DriveInfo d in allDrives)
        {
            r += string.Format("Drive {0}", d.Name);
            r += string.Format("  Drive type: {0}", d.DriveType);
            if (d.IsReady == true)
            {
                r += string.Format("  Volume label: {0}", d.VolumeLabel);
                r += string.Format("  File system: {0}", d.DriveFormat);
                r += string.Format("  Available space to current user:{0, 15} MB", d.AvailableFreeSpace/1024/1024);
                r += string.Format("  Total available space:          {0, 15} MB", d.TotalFreeSpace/1024/1024);
                r += string.Format("  Total size of drive:            {0, 15} MB",d.TotalSize/1024/1024);
            }
        }
       return r;
}

public static string AvailableFreeSpaceInMb(string letter)
{
       string r = "";
       System.IO.DriveInfo[] allDrives = System.IO.DriveInfo.GetDrives();

        foreach (System.IO.DriveInfo d in allDrives)
        {
            if(d.Name.ToLower().StartsWith(letter.ToLower()))
            {
                r+=string.Format("{0, 2}", d.AvailableFreeSpace/1024/1024);
            }
            
        }
       return r;
}
</script>

<%try{%>
hMailServer running:<%=System.Diagnostics.Process.GetProcessesByName("hMailServer").Count().ToString()%><br/>
External IP:<%=(new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches((new System.Net.WebClient()).DownloadString("http://checkip.dyndns.org/"))[0].ToString()%><br/>
Internal IP:<%=System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[0].ToString()%><br/>
Up Time:<%=GetUpTime()%><br/>
Available on C:<%=AvailableFreeSpaceInMb("C")%> MB<br/>
Available on D:<%=AvailableFreeSpaceInMb("d")%> MB<br/>
<%}catch(Exception e){}%>