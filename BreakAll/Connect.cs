using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;

namespace BreakAll
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;
		}

        void OnSolutionClosed()
        {
            _commandBreakAll.Delete();
            _commandBreakAll = null;

            _commandBreakNone.Delete();
            _commandBreakNone = null;
        }

        void OnSolutionOpened()
        {
            object[] contextGUIDS = new object[] { };
            Commands2 commands = (Commands2)_applicationObject.Commands;
            string debugMenuName = "Debug";

            //Place the command on the debug menu.
            //Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
            Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

            //Find the Debug command bar on the MenuBar command bar:
            CommandBarControl debugControl = menuBarCommandBar.Controls[debugMenuName];
            CommandBarPopup debugPopup = (CommandBarPopup)debugControl;

            //This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
            //  just make sure you also update the QueryStatus/Exec method to include the new command names.
            try
            {
                //Add a command to the Commands collection:
                _commandBreakAll = commands.AddNamedCommand2(_addInInstance, "BreakAll", "Set Class Breakpoint", "Sets a breakpoint for each member of each class in the current file", true, Type.Missing, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                _commandBreakNone = commands.AddNamedCommand2(_addInInstance, "BreakNone", "Delete All Breakpoints In File", "Deletes all breakpoints in the current file", true, Type.Missing, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                int position = 1;
                foreach (CommandBarControl command in debugPopup.CommandBar.Controls)
                {
                    position++;
                    if (command.accName.ToLower().Contains("disable all breakpoints"))
                    {
                        break;
                    }
                }

                //Add a control for the command to the tools menu:
                if ((_commandBreakAll != null) && (_commandBreakNone != null) && (debugPopup != null))
                {
                    _commandBreakAll.AddControl(debugPopup.CommandBar, position);
                    _commandBreakNone.AddControl(debugPopup.CommandBar, position);
                }

                _commandBreakAll.Bindings = "Text Editor::ctrl+d, z";
                _commandBreakNone.Bindings = "Text Editor::ctrl+d, x";
            }
            catch (System.ArgumentException)
            {
                //If we are here, then the exception is probably because a command with that name
                //  already exists. If so there is no need to recreate the command and we can 
                //  safely ignore the exception.
            }
        }

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
            _solutionEvents = _applicationObject.Events.SolutionEvents;
            _solutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler(OnSolutionOpened);
            _solutionEvents.AfterClosing += new _dispSolutionEvents_AfterClosingEventHandler(OnSolutionClosed);
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
                if (commandName == "BreakAll.Connect.BreakAll" || commandName == "BreakAll.Connect.BreakNone")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
            // Setup debug Output window.
            Window w = (Window)this._applicationObject.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            w.Visible = true;
            OutputWindow ow = (OutputWindow)w.Object;
            OutputWindowPane owp = ow.OutputWindowPanes.Add("BreakAll tracing");
            owp.Activate(); 
            
            handled = false;
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
				if(commandName == "BreakAll.Connect.BreakAll")
				{
                    if (this._applicationObject.ActiveDocument == null)
                    {
                        return;
                    }

                    List<CodeClass> classes = new List<CodeClass>();
                    RecursiveClassSearch(this._applicationObject.ActiveDocument.ProjectItem.FileCodeModel.CodeElements, classes);

                    foreach (CodeClass codeClass in classes)
                    {
                        foreach (object targetObject in codeClass.Members)
                        {
                            if (targetObject is CodeFunction)
                            {
                                CodeFunction method = targetObject as CodeFunction;
                                try
                                {
                                    this._applicationObject.Debugger.Breakpoints.Add(string.Empty, this._applicationObject.ActiveDocument.FullName, method.GetStartPoint(vsCMPart.vsCMPartBody).Line);
                                }
                                catch (Exception ex)
                                {
                                    owp.OutputString("\nException from Debugger.Breakpoints.Add: " + ex.Message);
                                }
                            }
                            else if (targetObject is CodeProperty)
                            {
                                CodeProperty property = targetObject as CodeProperty;
                                try
                                {
                                    this._applicationObject.Debugger.Breakpoints.Add(string.Empty, this._applicationObject.ActiveDocument.FullName, property.Getter.GetStartPoint(vsCMPart.vsCMPartBody).Line);
                                }
                                catch (Exception ex)
                                {
                                    owp.OutputString("\nException from Debugger.Breakpoints.Add: " + ex.Message);
                                }
                                try
                                {
                                    this._applicationObject.Debugger.Breakpoints.Add(string.Empty, this._applicationObject.ActiveDocument.FullName, property.Setter.GetStartPoint(vsCMPart.vsCMPartBody).Line);
                                }
                                catch (Exception ex)
                                {
                                    owp.OutputString("\nException from Debugger.Breakpoints.Add: " + ex.Message);
                                }
                            }
                        }
                    }

                    handled = true;
                    return;
				}
                else if (commandName == "BreakAll.Connect.BreakNone")
                {
                    if (this._applicationObject.ActiveDocument == null)
                    {
                        return;
                    }

                    List<Breakpoint> breakpointsInCurrentFile = new List<Breakpoint>();
                    foreach (Breakpoint breakpoint in _applicationObject.Debugger.Breakpoints)
                    {
                        if (breakpoint.File == this._applicationObject.ActiveDocument.FullName)
                        {
                            breakpointsInCurrentFile.Add(breakpoint);
                        }
                    }

                    foreach (Breakpoint breakpoint in breakpointsInCurrentFile)
                    {
                        breakpoint.Delete();
                    }

                    handled = true;
                    return;
                }
			}
		}
		private DTE2 _applicationObject;
		private AddIn _addInInstance;
        private SolutionEvents _solutionEvents;
        private Command _commandBreakAll;
        private Command _commandBreakNone;

        private static void RecursiveClassSearch(CodeElements elements, List<CodeClass> foundClasses)
        {
            foreach (CodeElement codeElement in elements)
            {
                if (codeElement is CodeClass)
                {
                    foundClasses.Add(codeElement as CodeClass);
                }
                RecursiveClassSearch(codeElement.Children, foundClasses);
            }
        }
	}
}