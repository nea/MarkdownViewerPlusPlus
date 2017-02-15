// NPP plugin platform for .Net v0.93.96 by Kasper B. Graversen etc.
using NppPluginNET.PluginInfrastructure;
using System.Text;

namespace Kbg.NppPluginNET.PluginInfrastructure
{
    public interface INotepadPPGateway
	{
		void FileNew();

		string GetCurrentFilePath();
		unsafe string GetFilePath(int bufferId);
		void SetCurrentLanguage(LangType language);
        /// <summary>
        /// Returns the response from NPPM_GETFILENAME 
        /// </summary>
        /// <returns></returns>
        string GetCurrentFileName();
        /// <summary>
        /// Returns the response from NPPM_GETCURRENTDIRECTORY 
        /// </summary>
        /// <returns></returns>
        string GetCurrentDirectory();
	}

	/// <summary>
	/// This class holds helpers for sending messages defined in the Msgs_h.cs file. It is at the moment
	/// incomplete. Please help fill in the blanks.
	/// </summary>
	public class NotepadPPGateway : INotepadPPGateway
	{
		private const int Unused = 0;

		public void FileNew()
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_MENUCOMMAND, Unused, NppMenuCmd.IDM_FILE_NEW);
		}

        /// <summary>
        /// Returns the response from NPPM_GETCURRENTDIRECTORY 
        /// </summary>
        /// <returns></returns>
        public string GetCurrentDirectory()
        {
            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETCURRENTDIRECTORY, 0, path);
            return path.ToString();
        }

        /// <summary>
        /// Returns the response from NPPM_GETFILENAME
        /// </summary>
        /// <returns></returns>
        public string GetCurrentFileName()
        {
            StringBuilder fileName = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETFILENAME, 0, fileName);
            return fileName.ToString();
        }

        /// <summary>
        /// Gets the path of the current document.
        /// </summary>
        public string GetCurrentFilePath()
		{
			var path = new StringBuilder(2000);
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_GETFULLCURRENTPATH, 0, path);
			return path.ToString();
		}

		/// <summary>
		/// Gets the path of the current document.
		/// </summary>
		public unsafe string GetFilePath(int bufferId)
		{
			var path = new StringBuilder(2000);
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_GETFULLPATHFROMBUFFERID, bufferId, path);
			return path.ToString();
		}

		public void SetCurrentLanguage(LangType language)
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_SETCURRENTLANGTYPE, Unused, (int) language);
		}
	}

	/// <summary>
	/// This class holds helpers for sending messages defined in the Resource_h.cs file. It is at the moment
	/// incomplete. Please help fill in the blanks.
	/// </summary>
	class NppResource
	{
		private const int Unused = 0;

		public void ClearIndicator()
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) Resource.NPPM_INTERNAL_CLEARINDICATOR, Unused, Unused);
		}
	}
}
