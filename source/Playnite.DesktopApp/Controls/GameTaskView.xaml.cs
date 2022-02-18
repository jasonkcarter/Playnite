using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Playnite.SDK.Models;
using System.ComponentModel;
using Playnite.Common;
using Playnite.Emulators;

namespace Playnite.DesktopApp.Controls
{
    /// <summary>
    /// Interaction logic for GameTaskView.xaml
    /// </summary>
    public partial class GameTaskView : UserControl, INotifyPropertyChanged
    {
        public bool IsCurrentEmulatorProfileCustom => GameTask.EmulatorProfileId?.StartsWith(CustomEmulatorProfile.ProfilePrefix) == true;

        public bool CanOverrideArgs
        {
            get
            {
                if (GameTask.EmulatorProfileId?.StartsWith(CustomEmulatorProfile.ProfilePrefix) == true)
                {
                    return true;
                }
                else
                {
                    var emu = Emulators.First(a => a.Id == GameTask.EmulatorId);
                    var emuProf = emu.BuiltinProfiles?.FirstOrDefault(a => a.Id == GameTask.EmulatorProfileId);
                    if (emuProf != null)
                    {
                        var def = EmulatorDefinition.GetProfile(emu.BuiltInConfigId, emuProf.BuiltInProfileName);
                        if (def?.ScriptStartup == false)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public bool ShowCustomEmulatorArgsRow
        {
            get
            {
                if (GameTask == null)
                {
                    return false;
                }

                return GameTask.Type == GameActionType.Emulator && GameTask.OverrideDefaultArgs && CanOverrideArgs;
            }
        }

        public bool ShowArgumentsRow
        {
            get
            {
                if (GameTask == null)
                {
                    return false;
                }

                return GameTask.Type == GameActionType.File;
            }
        }

        public bool ShowUsernameRow
        {
            get
            {
                if (GameTask == null)
                {
                    return false;
                }

                return GameTask.Type == GameActionType.File;
            }
        }

        public bool ShowPasswordRow
        {
            get
            {
                if (GameTask == null)
                {
                    return false;
                }

                return GameTask.Type == GameActionType.File;
            }
        }

        public bool ShowAdditionalArgumentsRow
        {
            get
            {
                if (GameTask == null)
                {
                    return false;
                }
                else if (GameTask.Type == GameActionType.Emulator && !CanOverrideArgs)
                {
                    return false;
                }
                else if (GameTask.Type == GameActionType.Emulator && !GameTask.OverrideDefaultArgs)
                {
                    return true;
                }

                return false;
            }
        }

        public bool ShowDefaultArgumentsRow
        {
            get
            {
                if (GameTask == null)
                {
                    return false;
                }

                if (GameTask.Type == GameActionType.Emulator && !CanOverrideArgs)
                {
                    return false;
                }
                else if (GameTask.Type == GameActionType.Emulator && !GameTask.OverrideDefaultArgs)
                {
                    return true;
                }

                return false;
            }
        }

        public bool ShowPathRow
        {
            get
            {
                if (GameTask == null)
                {
                    return false;
                }

                return GameTask.Type != GameActionType.Emulator;
            }
        }

        public bool ShowWorkingDirRow
        {
            get
            {
                if (GameTask == null)
                {
                    return false;
                }

                return GameTask.Type == GameActionType.File;
            }
        }

        public bool ShowEmulatorRow
        {
            get
            {
                if (GameTask == null)
                {
                    return false;
                }

                return GameTask.Type == GameActionType.Emulator;
            }
        }

        public bool ShowOverrideArgsRow
        {
            get
            {
                if (GameTask == null)
                {
                    return false;
                }

                if (GameTask.Type == GameActionType.Emulator && !CanOverrideArgs)
                {
                    return false;
                }

                return GameTask.Type == GameActionType.Emulator;
            }
        }

        public bool ShowTrackingPathRow
        {
            get
            {
                if (GameTask == null)
                {
                    return false;
                }

                return GameTask.TrackingMode == TrackingMode.Directory && GameTask.Type != GameActionType.Emulator;
            }
        }

        public bool ShowTrackingModeRow
        {
            get
            {
                if (GameTask == null)
                {
                    return false;
                }

                return (GameTask.Type == GameActionType.File || GameTask.Type == GameActionType.URL) && GameTask.IsPlayAction;
            }
        }

        public bool ShowScriptInput
        {
            get
            {
                return GameTask?.Type == GameActionType.Script;
            }
        }

        public GameAction GameTask
        {
            get
            {
                if (DataContext == null)
                {
                    return null;
                }
                else
                {
                    return ((GameAction)DataContext);
                }
            }
        }

        private string selectedEmulatorArguments;
        public string SelectedEmulatorArguments
        {
            get => selectedEmulatorArguments;
            set
            {
                selectedEmulatorArguments = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedEmulatorArguments)));
            }
        }

        public List<Emulator> Emulators
        {
            get
            {
                return (List<Emulator>)GetValue(EmulatorsProperty);
            }

            set
            {
                SetValue(EmulatorsProperty, value);
            }
        }

        public static readonly DependencyProperty EmulatorsProperty =
            DependencyProperty.Register(nameof(Emulators), typeof(List<Emulator>), typeof(GameTaskView));

        public bool ShowNameRow
        {
            get
            {
                return (bool)GetValue(ShowNameRowProperty);
            }

            set
            {
                SetValue(ShowNameRowProperty, value);
            }
        }

        public static readonly DependencyProperty ShowNameRowProperty =
            DependencyProperty.Register(nameof(ShowNameRow), typeof(bool), typeof(GameTaskView));

        public string DefaultSelectionDir
        {
            get
            {
                return (string)GetValue(DefaultSelectionDirProperty);
            }

            set
            {
                SetValue(DefaultSelectionDirProperty, value);
            }
        }

        public static readonly DependencyProperty DefaultSelectionDirProperty =
            DependencyProperty.Register(nameof(DefaultSelectionDir), typeof(string), typeof(GameTaskView));

        public event PropertyChangedEventHandler PropertyChanged;

        public GameTaskView()
        {
            InitializeComponent();
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            var path = SystemDialogs.SelectFile(Window.GetWindow(this), "*.*|*.*", DefaultSelectionDir);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            TextPath.Text = path;
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NotifyRowChange();
        }

        private void ComboTrackingMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ShowTrackingPathRow));
        }

        private void NotifyRowChange()
        {
            OnPropertyChanged(nameof(ShowArgumentsRow));
            OnPropertyChanged(nameof(ShowUsernameRow));
            OnPropertyChanged(nameof(ShowPasswordRow));
            OnPropertyChanged(nameof(ShowAdditionalArgumentsRow));
            OnPropertyChanged(nameof(ShowDefaultArgumentsRow));
            OnPropertyChanged(nameof(ShowPathRow));
            OnPropertyChanged(nameof(ShowWorkingDirRow));
            OnPropertyChanged(nameof(ShowEmulatorRow));
            OnPropertyChanged(nameof(ShowOverrideArgsRow));
            OnPropertyChanged(nameof(ShowCustomEmulatorArgsRow));
            OnPropertyChanged(nameof(ShowTrackingModeRow));
            OnPropertyChanged(nameof(ShowScriptInput));
        }

        private void CheckOverrideArgs_Checked(object sender, RoutedEventArgs e)
        {
            if (GameTask == null)
            {
                return;
            }

            NotifyRowChange();
            if (GameTask.OverrideDefaultArgs && !SelectedEmulatorArguments.IsNullOrEmpty() && GameTask.Arguments.IsNullOrEmpty())
            {
                GameTask.Arguments = $"{SelectedEmulatorArguments} {GameTask.AdditionalArguments}".Trim();
            }
        }

        private void ComboEmulator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GameTask == null)
            {
                return;
            }

            if (Emulators == null || Emulators.Count == 0)
            {
                return;
            }

            if (GameTask?.EmulatorId != Guid.Empty && Emulators.Any(a => a.Id == GameTask?.EmulatorId))
            {
                ComboEmulatorConfig.SelectedItem = Emulators.First(a => a.Id == GameTask.EmulatorId).CustomProfiles?.FirstOrDefault();
            }
        }

        private void ComboEmulatorConfig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GameTask == null)
            {
                return;
            }

            if (Emulators == null || Emulators.Count == 0)
            {
                SelectedEmulatorArguments = string.Empty;
                return;
            }

            if (GameTask?.EmulatorId != Guid.Empty && Emulators.Any(a => a.Id == GameTask?.EmulatorId))
            {
                var emulator = Emulators.First(a => a.Id == GameTask.EmulatorId);
                var emulatorProfile = emulator.AllProfiles?.FirstOrDefault(a => a.Id == GameTask.EmulatorProfileId);
                if (emulatorProfile == null)
                {
                    SelectedEmulatorArguments = string.Empty;
                }
                else
                {
                    if (emulatorProfile is CustomEmulatorProfile customProfile)
                    {
                        SelectedEmulatorArguments = customProfile.Arguments;
                    }
                    else if (emulatorProfile is BuiltInEmulatorProfile builtInProfile)
                    {
                        if (builtInProfile.OverrideDefaultArgs)
                        {
                            SelectedEmulatorArguments = builtInProfile.CustomArguments;
                        }
                        else
                        {
                            var def = EmulatorDefinition.GetProfile(emulator.BuiltInConfigId, builtInProfile.BuiltInProfileName);
                            if (def?.ScriptStartup == false)
                            {
                                SelectedEmulatorArguments = def.StartupArguments;
                            }
                            else
                            {
                                SelectedEmulatorArguments = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        SelectedEmulatorArguments = string.Empty;
                    }
                }
            }
            else
            {
                SelectedEmulatorArguments = string.Empty;
            }

            NotifyRowChange();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            NotifyRowChange();
        }

        private void ListBoxAffinity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(GameTask == null) { return; }
            foreach(int core in e.AddedItems)
            {
                GameTask.ProcessorAffinity += (int)Math.Pow(2, core - 1);
            }
            foreach (int core in e.RemovedItems)
            {
                GameTask.ProcessorAffinity -= (int)Math.Pow(2, core - 1);
            }
            NotifyRowChange();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (GameTask == null)
            {
                return;
            }
            // Default processor affinity to all cores
            int allProcessorsMask = (int)Math.Pow(2, Environment.ProcessorCount) - 1;
            if (GameTask.ProcessorAffinity == default(int) || GameTask.ProcessorAffinity < 0)
            {
                GameTask.ProcessorAffinity = allProcessorsMask;
            }
            // If CPU count is reduced, remove any excess bits
            else if (GameTask.ProcessorAffinity > allProcessorsMask)
            {
                GameTask.ProcessorAffinity &= allProcessorsMask;
            }
            for (int core = 1; core <= Environment.ProcessorCount; core++)
            {
                ListBoxAffinity.Items.Add(core);
                int coreMask = (int)Math.Pow(2, core - 1);
                if ((GameTask.ProcessorAffinity & coreMask) == coreMask)
                {
                    ListBoxAffinity.SelectedItems.Add(core);
                }
            }
            ListBoxAffinity.SelectionChanged += ListBoxAffinity_SelectionChanged;
        }

        private void ListBoxAffinity_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }
    }
}
