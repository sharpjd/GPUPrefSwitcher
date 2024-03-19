using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace GPUPrefSwitcher
{
    //TODO: Refactor this to use XDocument and XElement because it's literally just better
        //They support directly reading line number

    /// <summary>
    /// YOU SHOULD BE USING <see cref="AppEntrySaveHandler"/> to manipulate data from other classes. Reads the XML which contains AppEntry data. Must be instantiated.
    /// 
    /// Warning: many private/internal functions are not thread safe
    /// </summary>
    public class PreferencesXML
    {
        /*
        public class ThreadSafeXmlDoc
        {
            //private XmlDocument xmlDocument = new XmlDocument();
            private SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);
            private XmlDocument xmlDocument = new();

            public void Load(string path)
            {
                SemaphoreSlim.Wait();
                try
                {
                    xmlDocument.Load(path);
                }
                finally { SemaphoreSlim.Release(); }
            }

            public void Save(string path)
            {
                SemaphoreSlim.Wait();
                try
                {
                    xmlDocument.Save(path);
                }
                finally { SemaphoreSlim.Release(); }
            }

            public XmlNodeList GetElementsByTagName(string tagName)
            {
                SemaphoreSlim.Wait();
                try
                {
                    return xmlDocument.GetElementsByTagName(tagName);
                }
                finally { SemaphoreSlim.Release() ; }
            }
        }
        */
        

        public static readonly string XML_PREFERENCES_PATH = Program.SavedDataPath + "Preferences.xml";
        private XmlDocument xmlDocument = new();

        internal PreferencesXML()
        {

            try
            {
                xmlDocument.Load(XML_PREFERENCES_PATH);
            }
            catch (XmlException)
            {
                Logger.inst.ErrorLog("XML Preferences document is not well formed, run it through a syntax checker and make sure everything is under a root element named AppsList.");
                throw;
            }
            catch (Exception)
            {
                Logger.inst.ErrorLog("Generic error when loading XML Preferences document.");
                throw;
            }

            string errors = GetAllCriticalXmlAppEntryErrors();
            if (errors != NO_ERRORS_STRING)
            {
                Logger.inst.ErrorLog(errors);
                throw new InvalidOperationException("XML Preferences document contains critical AppEntry errors.");
            }

            //you can also add/remove new XML elements from here by building, running the GUI once, removing the code, building again
            /* Example:
            XmlNodeList xmlAppEntries = xmlDocument.GetElementsByTagName(XML_APP_ENTRY);
            foreach (XmlNode xmlAppEntry in xmlAppEntries)
            {
                XmlNode xmlGpuPreference = xmlAppEntry.SelectSingleNode(XML_GPU_PREFERENCE);
                {
                    xmlGpuPreference.AppendChild(xmlDocument.CreateElement("RunOnBattery"));
                    xmlGpuPreference.AppendChild(xmlDocument.CreateElement("RunPluggedIn"));
                    
                }
            }
            xmlDocument.Save(XML_PREFERENCES_PATH);
            */
        }

        /*
        internal void ReloadXML()
        {
            try
            {
                xmlDocument.Load(XML_PREFERENCES_PATH);
            }
            catch (Exception)
            {
                throw;
            }
        }
        */

        internal const string XML_APP_PATH = "Path";
        internal const string XML_APP_ENTRY = "AppEntry";
        internal const string XML_GPU_PREFERENCE = "GPUPreference";
        internal const string XML_PLUGGED_IN = "PluggedIn";
        internal const string XML_ON_BATTERY = "OnBattery";
        internal const string XML_ATTR_ENABLESWITCHER = "enableSwitcher";
        internal const string XML_ATTR_ENABLEFILESWAPPER = "enableFileSwapper";
        internal const string XML_ATTR_SWAPPATHSTATUS = "state";
        internal const string XML_FILE_SWAPPER = "FileSwapper";
        internal const string XML_SWAP_PATH = "SwapPath";
        internal const string XML_PENDING_ADD = "PendingAddToRegistry";
        internal const string XML_RUN_ON_BATTERY_PATH = "RunOnBattery";
        internal const string XML_RUN_PLUGGED_IN_PATH = "RunPluggedIn";
        internal const string XML_SEEN_IN_REGISTRY = "SeenInRegistry";

        //ugly, but necessary; you can place the cursor line at the leftmost to highlight each corresponding constant
        internal readonly string[] VALID_ATTRIBUTES =
        [
            XML_APP_PATH, 
            XML_APP_ENTRY,
            XML_GPU_PREFERENCE,
            XML_PLUGGED_IN,
            XML_ON_BATTERY,
            XML_ATTR_ENABLESWITCHER,
            XML_ATTR_ENABLEFILESWAPPER,
            XML_ATTR_SWAPPATHSTATUS,
            XML_FILE_SWAPPER,
            XML_SWAP_PATH,
            XML_PENDING_ADD,
            XML_RUN_ON_BATTERY_PATH,
            XML_RUN_PLUGGED_IN_PATH,
            XML_SEEN_IN_REGISTRY
        ];

        #region Accessors

        //can't be an ienumerable otherwise it might cause inconsistent xml state
        /// <summary>
        /// 
        /// </summary>
        /// <returns>A readonly list of AppEntries; you must call the modify or save functions to actually change these in the XML document.</returns>
        internal List<AppEntry> GetAppEntries()
        {

            //try
            //{
                //semaphoreSlim.Wait();

                xmlDocument.Load(XML_PREFERENCES_PATH); //performance hog?

                XmlNodeList xmlAppEntries = xmlDocument.GetElementsByTagName(XML_APP_ENTRY);

                List<AppEntry> appEntries = new List<AppEntry>();

                foreach (XmlNode xmlAppEntry in xmlAppEntries)
                {

                    string appPath;
                    bool enableSwitcher;
                    int gpuPrefOnBattery;
                    int gpuPrefPluggedIn;
                    bool enableFileSwapper;
                    string runOnBatteryPath;
                    string runPluggedInPath;
                    bool seenInRegistry;

                    appPath = xmlAppEntry.SelectSingleNode(XML_APP_PATH).InnerText;
                    //Debug.WriteLine(appPath);
                    XmlNode xmlGpuPreference = xmlAppEntry.SelectSingleNode(XML_GPU_PREFERENCE);
                    enableSwitcher = bool.Parse(xmlGpuPreference.Attributes[XML_ATTR_ENABLESWITCHER].Value);
                    gpuPrefOnBattery = int.Parse(xmlGpuPreference.SelectSingleNode(XML_ON_BATTERY).InnerText);
                    gpuPrefPluggedIn = int.Parse(xmlGpuPreference.SelectSingleNode(XML_PLUGGED_IN).InnerText);
                    enableFileSwapper = bool.Parse(xmlGpuPreference.Attributes[XML_ATTR_ENABLEFILESWAPPER].Value);
                    runOnBatteryPath = xmlGpuPreference.SelectSingleNode(XML_RUN_ON_BATTERY_PATH).InnerText;
                    runPluggedInPath = xmlGpuPreference.SelectSingleNode(XML_RUN_PLUGGED_IN_PATH).InnerText;
                    seenInRegistry = bool.Parse(xmlAppEntry.SelectSingleNode(XML_SEEN_IN_REGISTRY).InnerText);

                    string[] swapPaths = GetAppEntryFileSwapperPaths(xmlAppEntry);

                    bool pendingAddToRegistry = false;
                    if (xmlAppEntry.SelectSingleNode(XML_PENDING_ADD) != null)
                    {
                        pendingAddToRegistry = true;
                    }

                    string[] fileSwapPathStatus = new string[swapPaths.Length];
                    XmlNodeList swapPathNodes = GetAppEntryFileSwapperXmlNodes(xmlAppEntry);
                    for (int i = 0; i < swapPaths.Length; i++)
                    {
                        string state = swapPathNodes[i].Attributes["state"].Value;
                        fileSwapPathStatus[i] = state;
                    }

                    appEntries.Add(
                            new AppEntry()
                            {
                                AppPath = appPath,
                                EnableSwitcher = enableSwitcher,
                                GPUPrefOnBattery = gpuPrefOnBattery,
                                GPUPrefPluggedIn = gpuPrefPluggedIn,
                                EnableFileSwapper = enableFileSwapper,
                                FileSwapperPaths = swapPaths,
                                SwapperStates = PowerLineStatusConversions.StringArrToPowerLineStatusArr(fileSwapPathStatus),
                                PendingAddToRegistry = pendingAddToRegistry,
                                RunOnBatteryPath = runOnBatteryPath,
                                RunPluggedInPath = runPluggedInPath,
                                SeenInRegistry = seenInRegistry,
                            }
                        );
                }
                return appEntries;
            /*
            } finally
            {
                semaphoreSlim.Release();
            }
            */

        }

        /*
        public IEnumerable<string> GetAppPaths()
        {
            xmlDocument.Load(XML_PREFERENCES_PATH);

            XmlNodeList pathEntries = xmlDocument.GetElementsByTagName(XML_APP_PATH);

            foreach (XmlNode pathEntryNode in pathEntries)
            {
                yield return pathEntryNode.InnerText;
            }
        }
        */

        internal string[] GetAppEntryFileSwapperPaths(XmlNode appEntry)
        {
            XmlNode fileSwapper = appEntry.SelectSingleNode(XML_FILE_SWAPPER);
            var nodes = fileSwapper.SelectNodes(XML_SWAP_PATH);
            //Debug.WriteLine(nodes.Count);
            var strings = new string[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                strings[i] = nodes[i].InnerText;
            }

            return strings;
        }

        internal XmlNodeList GetAppEntryFileSwapperXmlNodes(XmlNode appEntry)
        {
            XmlNode fileSwapper = appEntry.SelectSingleNode(XML_FILE_SWAPPER);
            return fileSwapper.SelectNodes(XML_SWAP_PATH);
        }

        internal XmlNode AppEntryNodeByAppPath(string path)
        {
            XmlNodeList xmlAppEntries = xmlDocument.GetElementsByTagName(XML_APP_ENTRY);

            foreach (XmlNode xmlAppEntry in xmlAppEntries)
            {
                string appPath = xmlAppEntry.SelectSingleNode(XML_APP_PATH).InnerText;

                if (path == appPath) return xmlAppEntry;

            }
            return null;
        }

        #endregion Accessors

        #region XML Writers
        /// <summary>
        /// Adds the specified app path to the XML with the following data. Does not check if the app path you're trying to add actually exists in the Registry, but does check if an entry with the same AppPath already exists
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="InvalidOperationException">If an AppEntry with the same AppPath already exists in the data store</exception>
        internal void AddAppEntryAndSave(AppEntry appEntry)
        {

            //try
            //{
                xmlDocument.Load(XML_PREFERENCES_PATH);

                //check for duplicates
                if (GetAppEntries().Any(entry => entry.AppPath == appEntry.AppPath))
                {
                    throw new InvalidOperationException($"Tried to add an AppEntry with an AppPath that already exists in the data store: {appEntry.AppPath} — this is undefined behavior.");
                }

                XmlNode root = xmlDocument.DocumentElement;
                {
                    XmlElement xmlAppEntry = xmlDocument.CreateElement(XML_APP_ENTRY);
                    XmlElement xmlPath = xmlDocument.CreateElement(XML_APP_PATH);
                    {
                        xmlPath.InnerText = appEntry.AppPath;
                        {
                            XmlElement xmlGpuPref = xmlDocument.CreateElement(XML_GPU_PREFERENCE);
                            {
                                xmlGpuPref.SetAttribute(XML_ATTR_ENABLESWITCHER, appEntry.EnableSwitcher.ToString());
                                xmlGpuPref.SetAttribute(XML_ATTR_ENABLEFILESWAPPER, appEntry.EnableFileSwapper.ToString());

                                XmlElement runOnBattery = xmlDocument.CreateElement(XML_RUN_ON_BATTERY_PATH);
                                XmlElement runPluggedIn = xmlDocument.CreateElement(XML_RUN_PLUGGED_IN_PATH);
                                xmlGpuPref.AppendChild(runOnBattery);
                                xmlGpuPref.AppendChild(runPluggedIn);
                            }
                            XmlElement xmlPluggedIn = xmlDocument.CreateElement(XML_PLUGGED_IN);
                            {
                                xmlPluggedIn.InnerText = appEntry.GPUPrefPluggedIn.ToString();
                            }
                            //xmlPluggedIn.InnerText = defaultPluggedin;        
                            XmlElement xmlOnBattery = xmlDocument.CreateElement(XML_ON_BATTERY);
                            {
                                xmlOnBattery.InnerText = appEntry.GPUPrefOnBattery.ToString();
                            }
                            XmlElement xmlFileSwapper = xmlDocument.CreateElement(XML_FILE_SWAPPER);


                            //xmlOnBattery.InnerText = defaultOnBattery;
                            root.AppendChild(xmlAppEntry);
                            {
                                xmlAppEntry.AppendChild(xmlPath);
                                xmlAppEntry.AppendChild(xmlGpuPref);
                                xmlAppEntry.AppendChild(xmlFileSwapper);
                            }
                            xmlGpuPref.AppendChild(xmlPluggedIn);
                            xmlGpuPref.AppendChild(xmlOnBattery);

                            if (appEntry.PendingAddToRegistry)
                            {
                                XmlElement pendingAdd = xmlDocument.CreateElement(XML_PENDING_ADD);
                                xmlAppEntry.AppendChild(pendingAdd);
                            }

                            XmlElement seenInRegistry = xmlDocument.CreateElement(XML_SEEN_IN_REGISTRY);
                            seenInRegistry.InnerText = appEntry.SeenInRegistry.ToString();
                            xmlAppEntry.AppendChild(seenInRegistry);
                        }
                    }

                    xmlDocument.Save(XML_PREFERENCES_PATH);
                }
                /*
            }
            finally
            {
                semaphoreSlim.Release();
            }
                */
        }

        /// <summary>
        /// Delete the <see cref="AppEntry"/> with the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if the <see cref="AppEntry"/> with the provided path is found; false otherwise</returns>
        /// <exception cref="XMLHelperException"></exception>
        internal bool TryDeleteAppEntryAndSave(string path)
        {
            xmlDocument.Load(XML_PREFERENCES_PATH);

            XmlNode xmlAppEntry = AppEntryNodeByAppPath(path);
            if (xmlAppEntry == null) { return false; }

            xmlDocument.DocumentElement.RemoveChild(xmlAppEntry);

            xmlDocument.Save(XML_PREFERENCES_PATH);

            return true;            
        }

        internal void ModifyAppEntryAndSave(string path, AppEntry newAppEntry) //TODO file swapper functionality
        {

            xmlDocument.Load(XML_PREFERENCES_PATH);

            XmlNode xmlAppEntry = AppEntryNodeByAppPath(path);
            if (xmlAppEntry == null) //I believe an exception is better than a Try function here because it results in less code assuming we get the logic right in the first place
            {
                throw new XMLHelperException($"ModifyAppEntry: AppEntry with the specified path not found in data store: {path}");
            }

            string newAppPath = newAppEntry.AppPath;
            string newEnableSwitcher = newAppEntry.EnableSwitcher.ToString().ToLower();
            string newGPUPrefPluggedIn = newAppEntry.GPUPrefPluggedIn.ToString();
            string newGPUPrefOnBattery = newAppEntry.GPUPrefOnBattery.ToString();
            string newEnableFileSwapper = newAppEntry.EnableFileSwapper.ToString().ToLower();
            string newRunOnBatteryPath = newAppEntry.RunOnBatteryPath;
            string newRunPluggedInPath = newAppEntry.RunPluggedInPath;
            string newSeenInRegistry = newAppEntry.SeenInRegistry.ToString().ToLower();

            string[] newFileSwapperPaths = newAppEntry.FileSwapperPaths;

            if (newAppPath == null)
                throw new InvalidOperationException("AppEntry passed in had a null AppPath value");
            if (newEnableSwitcher == null)
                throw new InvalidOperationException($"AppEntry passed in had a null EnableSwitcher value (by AppPath {path})");
            if (newGPUPrefPluggedIn == null)
                throw new InvalidOperationException($"AppEntry passed in had a null GPUPrefPluggedIn value (by AppPath {path})");
            if (newGPUPrefOnBattery == null)
                throw new InvalidOperationException($"AppEntry passed in had a null GPUPrefOnBattery value (by AppPath {path})");
            if (newEnableFileSwapper == null)
                throw new InvalidOperationException($"AppEntry passed in had a null EnableFileSwapper value (by AppPath{path}");
            if (newRunOnBatteryPath == null)
                throw new InvalidOperationException($"AppEntry passed in had a null RunOnBatteryPath value (by AppPath{path}");
            if (newRunPluggedInPath == null)
                throw new InvalidOperationException($"AppEntry passed in had a null RunPluggedInPath value (by AppPath{path}");
            if (newSeenInRegistry == null)
                throw new InvalidOperationException($"AppEntry passed in had a null SeenInRegistry value (by AppPath{path}");

            //try
            {
                xmlAppEntry.SelectSingleNode(XML_APP_PATH).InnerText = newAppPath;

                xmlAppEntry.SelectSingleNode(XML_SEEN_IN_REGISTRY).InnerText = newSeenInRegistry;

                XmlNode gpuPreference = xmlAppEntry.SelectSingleNode(XML_GPU_PREFERENCE);
                gpuPreference.Attributes[XML_ATTR_ENABLESWITCHER].Value = newEnableSwitcher;
                gpuPreference.Attributes[XML_ATTR_ENABLEFILESWAPPER].Value = newEnableFileSwapper;
                {
                    gpuPreference.SelectSingleNode(XML_PLUGGED_IN).InnerText = newGPUPrefPluggedIn;
                    gpuPreference.SelectSingleNode(XML_ON_BATTERY).InnerText = newGPUPrefOnBattery;

                    gpuPreference.SelectSingleNode(XML_RUN_PLUGGED_IN_PATH).InnerText = newRunPluggedInPath;
                    gpuPreference.SelectSingleNode(XML_RUN_ON_BATTERY_PATH).InnerText = newRunOnBatteryPath;

                }

                //TODO this is jank, I think Switcher.cs needs to not have access to this and only access to AppEntrySaveHandler instead
                if (newAppEntry.PendingAddToRegistry)
                {
                    XmlElement pendingAddToRegistry = xmlDocument.CreateElement(XML_PENDING_ADD);
                    if (xmlAppEntry.SelectSingleNode(XML_PENDING_ADD) == null)
                    {
                        xmlAppEntry.AppendChild(pendingAddToRegistry);
                    }
                }
                else
                {
                    var pendingAdd = xmlAppEntry.SelectSingleNode(XML_PENDING_ADD);
                    if (pendingAdd != null)
                    {
                        xmlAppEntry.RemoveChild(pendingAdd);
                    }
                }

                //replace the SwapPath's
                XmlNode fileSwapper = xmlAppEntry.SelectSingleNode(XML_FILE_SWAPPER);
                fileSwapper.RemoveAll();
                {
                    /*
                    foreach(string p in newFileSwapperPaths)
                    {
                        XmlElement xmlElement = xmlDocument.CreateElement(XML_SWAP_PATH);
                        xmlElement.InnerText = p;
                        fileSwapper.AppendChild(xmlElement);
                    }
                    */
                    for (int i = 0; i < newFileSwapperPaths.Count(); i++)
                    {
                        string newFileSwapperPath = newFileSwapperPaths[i];

                        XmlElement xmlElement = xmlDocument.CreateElement(XML_SWAP_PATH);

                        xmlElement.SetAttribute(XML_ATTR_SWAPPATHSTATUS, PowerLineStatusConversions.PowerLineStatusToOfflineOrOnline(newAppEntry.SwapperStates[i]));//TODO: does this gauruntee order?

                        xmlElement.InnerText = newFileSwapperPath;

                        fileSwapper.AppendChild(xmlElement);//remember to append it!!!
                    }
                }

                xmlDocument.Save(XML_PREFERENCES_PATH);
            }/* catch (NullReferenceException)
            {
                throw new XmlException($"An error occured while trying to modify AppEntry with AppPath {path}; check if the entry is malformed in Preferences.xml");
            }*/
        }

        #endregion XML Writers

        #region XML Marshalling
        public static readonly string NO_ERRORS_STRING = string.Empty;
        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the input <see cref="XmlNode"/> is not an AppEntry node
        /// Critical app entry errors include:
        /// - Missing malformed, or invalid elements and attributes
        /// Does not count:
        /// - Nonexistent target file paths
        /// 
        /// </summary>
        /// <returns><see cref="NO_ERRORS_STRING"/> if there is no error; otherwise, a multiline string that lists all the errors of an AppEntry (such as missing info or invalid values)</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        private string IdentifyCriticalAppEntryErrors(XmlElement appEntryNode, int linePosition)
        {
            if (appEntryNode.Name != XML_APP_ENTRY) throw new ArgumentException("Passed an XmlNode that isn't an 'AppEntry' node");

            StringBuilder sb = new(NO_ERRORS_STRING);

            string appPath;
            bool enableSwitcher;
            int gpuPrefOnBattery;
            int gpuPrefPluggedIn;
            bool enableFileSwapper;
            string runOnBatteryPath;
            string runPluggedInPath;
            bool seenInRegistry;

            
            XmlNode xmlAppPath = appEntryNode.SelectSingleNode(XML_APP_PATH);
            if (xmlAppPath == null) { sb.AppendLine($"Missing or malformed element: {XML_APP_PATH}"); }
            appPath = xmlAppPath.InnerText;

            XmlNode xmlGpuPreference = appEntryNode.SelectSingleNode(XML_GPU_PREFERENCE);
            if (xmlGpuPreference is null) { sb.AppendLine($"Missing or malformed element: {XML_GPU_PREFERENCE}"); }
            else
            {
                
                XmlAttribute xmlEnableSwitcherAttr;
                xmlEnableSwitcherAttr = xmlGpuPreference.Attributes[XML_ATTR_ENABLESWITCHER];
                if(xmlEnableSwitcherAttr is null) { sb.AppendLine($"Missing {XML_GPU_PREFERENCE} element attribute: {XML_ATTR_ENABLESWITCHER}"); }
                else
                {
                    try
                    {
                        enableSwitcher = bool.Parse(xmlEnableSwitcherAttr.Value);
                    }
                    catch (FormatException) { sb.AppendLine($"Invalid {XML_ATTR_ENABLESWITCHER} attribute value; must be true/false"); }
                }

                XmlAttribute xmlEnableFileSwapper;
                xmlEnableFileSwapper = xmlGpuPreference.Attributes[XML_ATTR_ENABLEFILESWAPPER];
                if (xmlEnableFileSwapper is null) { sb.AppendLine($"Missing {XML_ATTR_ENABLEFILESWAPPER} element attribute: {XML_ATTR_ENABLEFILESWAPPER}"); }
                else
                {
                    try
                    {
                        enableFileSwapper = bool.Parse(xmlEnableFileSwapper.Value);
                    }
                    catch (FormatException) { sb.AppendLine($"Invalid {XML_ATTR_ENABLEFILESWAPPER} attribute value; must be true/false"); }
                }

                XmlNode xmlOnBattery;
                xmlOnBattery = xmlGpuPreference.SelectSingleNode(XML_ON_BATTERY);
                if (xmlOnBattery is null) { sb.AppendLine($"Missing or malformed element: {XML_ON_BATTERY}"); }
                try
                {
                    gpuPrefOnBattery = int.Parse(xmlOnBattery.InnerText);
                    //the duplicate sb.AppendLine is okay because it won't fire if FormatException is thrown
                    if(gpuPrefOnBattery < -1 || gpuPrefOnBattery > 13) { sb.AppendLine($"Invalid {XML_GPU_PREFERENCE} value; must be an integer in the range [-1, 13]"); }
                }
                catch (FormatException) { sb.AppendLine($"Invalid {XML_GPU_PREFERENCE} value; must be an integer in the range [-1, 13]"); }

                XmlNode xmlPluggedIn;
                xmlPluggedIn = xmlGpuPreference.SelectSingleNode(XML_PLUGGED_IN);
                if (xmlPluggedIn is null) { sb.AppendLine($"Missing or malformed element: {XML_PLUGGED_IN}"); }
                try
                {
                    gpuPrefPluggedIn = int.Parse(xmlPluggedIn.InnerText);
                    //the duplicate sb.AppendLine is okay because it won't fire if FormatException is thrown
                    if (gpuPrefPluggedIn < -1 || gpuPrefPluggedIn > 13) { sb.AppendLine($"Invalid {XML_GPU_PREFERENCE} value; must be an integer in the range [-1, 13]"); }
                }
                catch (FormatException) { sb.AppendLine($"Invalid {XML_GPU_PREFERENCE} value; must be an integer in the range [-1, 13]"); }

                
                XmlNode xmlRunOnBattery = xmlGpuPreference.SelectSingleNode(XML_RUN_ON_BATTERY_PATH);
                if(xmlRunOnBattery is null) { sb.AppendLine($"Missing or malformed element: {XML_RUN_ON_BATTERY_PATH}"); }
                try
                {
                    runOnBatteryPath = xmlRunOnBattery.InnerText;
                } catch (Exception) { throw new NotImplementedException(); }//not implemented or necessary yet

                XmlNode xmlRunPluggedIn = xmlGpuPreference.SelectSingleNode(XML_RUN_PLUGGED_IN_PATH);
                if (xmlRunPluggedIn is null) { sb.AppendLine($"Missing or malformed element: {XML_RUN_PLUGGED_IN_PATH}"); }
                try
                {
                    runPluggedInPath = xmlRunOnBattery.InnerText;
                }
                catch (Exception) { throw new NotImplementedException(); }//not implemented or necessary yet

            }

            XmlNode xmlFileSwapper = appEntryNode.SelectSingleNode(XML_FILE_SWAPPER);
            if(xmlFileSwapper is null) { sb.AppendLine($"Missing or malformed element: {XML_FILE_SWAPPER}"); }
            var xmlNodes = xmlFileSwapper.SelectNodes(XML_SWAP_PATH);
            if(xmlNodes.Count!=0)
            {
                foreach(XmlNode xmlNode in xmlNodes)
                {
                    
                    string fileSwapperPath = xmlNode.InnerText;
                    /* Not really an error, if it doesn't exist, the Switcher will gloss over it 
                    var file = new FileInfo(fileSwapperPath);
                    if (!file.Exists) { sb.AppendLine($"FileSwapper target filepath does not exist: {fileSwapperPath}"); }
                    */

                    XmlAttribute state = xmlNode.Attributes[XML_ATTR_SWAPPATHSTATUS];
                    if(state==null) { sb.AppendLine($"Missing {XML_SWAP_PATH} element attribute: {XML_ATTR_SWAPPATHSTATUS}"); }
                    string state_str = state.InnerText.ToLower();
                    if (!(state_str == "online" || state_str == "offline"))
                    {
                        sb.AppendLine($"Invalid SwapPath {XML_ATTR_SWAPPATHSTATUS} attribute value for SwapPath {fileSwapperPath}; must be online/offline (warning: either backup the stored files (find via the GUI), or examine the file contents first and replace with the correct status to prevent data loss)");
                    }
                    
                }
            }

            XmlNode xmlSeenInRegistry = appEntryNode.SelectSingleNode(XML_SEEN_IN_REGISTRY);
            if(xmlSeenInRegistry is null) 
                { sb.AppendLine($"Missing or malformed element: {XML_SEEN_IN_REGISTRY}"); }
            else
                try
                {
                    seenInRegistry = bool.Parse(xmlSeenInRegistry.InnerText);
                } catch (FormatException) { sb.AppendLine($"Invalid {XML_SEEN_IN_REGISTRY} value; must be true/false (you can put either true/false here, it will not break behavior or overwrite existing registry entires)"); }

            if (sb.ToString() != NO_ERRORS_STRING)
            {
                string xmlDocumentToString = xmlDocument.OuterXml;
                sb.Insert(0, $"The AppEntry node starting on line {linePosition} has the following errors:\n");
            }

            return sb.ToString();
        }

        

        /// <summary>
        /// Concatenates all invalid entries using <see cref="IdentifyAppEntryErrors(XmlNode)". Returns <see cref="NO_ERRORS_STRING"/> if no errors are identified/>
        /// </summary>
        /// <returns></returns>
        public string GetAllCriticalXmlAppEntryErrors()
        {
            StringBuilder sb = new(NO_ERRORS_STRING);
            //apparently you can call SelectNodes() from xmlDocument which returns nothing, so you have to reference DocumentElement first...
            var appEntryNodes = xmlDocument.DocumentElement.SelectNodes(XML_APP_ENTRY);
            for(int i = 0; i < appEntryNodes.Count; i++)
            {
                XmlNode node = appEntryNodes[i];

                string errorString = IdentifyCriticalAppEntryErrors((XmlElement)node, GetLineNumberForAppEntry(i));
                if (errorString != NO_ERRORS_STRING)
                {
                    sb.AppendLine(errorString);
                }
            }
            return sb.ToString();

            int GetLineNumberForAppEntry(int index)
            {
                //string xmlContent = xmlDocument.OuterXml; //nope, this erases all line info
                XDocument xDocument = XDocument.Load(XML_PREFERENCES_PATH, LoadOptions.SetLineInfo);

                // Find all 'AppEntry' elements
                var appEntryElements = xDocument.Root.Descendants("AppEntry").ToList();

                XElement e = appEntryElements[0];
                for(int i = 1; i <= index; i++)
                {
                    e = appEntryElements[i];
                }

                return (e as IXmlLineInfo).LineNumber;
            }
        }

        //TODO: put this check as a button somewhere (e.g. the save button)? having multiple duplicate paths is undefined behavior...
        void CheckForDuplicatePaths()
        {

            var appEntries = GetAppEntries();
            var appPaths = from appEntry in appEntries select appEntry.AppPath;

            List<string> traversedAppPaths = new List<string>();

            foreach (string s in appPaths)
            {
                if (traversedAppPaths.Any(entry => entry == s)) //check for duplicate AppPath
                {
                    throw new InvalidOperationException($"Duplicate AppPaths in the AppEntry save: {s} — this is undefined state and behavior");
                }
                //should come after the duplicate entry check
                traversedAppPaths.Add(s);
            }
        }

        internal void DebugPrintXML()
        {
            XmlNodeList nodeList = xmlDocument.GetElementsByTagName(XML_APP_ENTRY);

            foreach (XmlElement e in nodeList)
            {
                Debug.WriteLine(e.ChildNodes.Count);
            }

            Debug.WriteLine(nodeList);
        }

        #endregion XML Marshalling

    }

    class XMLHelperException : Exception
    {
        public XMLHelperException(string message) : base(message)
        {
        }
    }

    
}
