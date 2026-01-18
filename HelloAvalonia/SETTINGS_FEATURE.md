# Settings Feature - Database Path Configuration

## ✅ Feature Implemented

You can now configure the database path through the UI!

## 🎯 How to Use

### 1. **Access Settings**
- Look for the **"⚙️ Paramètres"** button in the **top-right corner** of the main window
- Click it to open the settings dialog

### 2. **Configure Database Path**
In the settings dialog:
1. Click **"Parcourir..."** (Browse) button
2. Navigate to your main POS application's database file
3. Select the **pos.db** file
4. Click **"Tester la connexion"** (Test connection) to verify it works
5. Click **"Enregistrer"** (Save) to save the path

### 3. **Restart Application**
- Close and restart your invoice app
- It will now copy data from the new database location

## 📁 Database Path Storage

The selected path is saved in:
```
HelloAvalonia/database/db_path.txt
```

## 🔄 How It Works

### On App Startup:
1. App reads saved database path from `database/db_path.txt`
2. If not found, uses default: `C:\ProgramData\Aronium\SimplePos\pos.db`
3. **Copies** the database file to local `database/pos.db`
4. Works with the local copy (safe, no conflicts!)

### Benefits:
- ✅ No conflicts with main app
- ✅ Fast local access
- ✅ Fresh data every session
- ✅ User controls database location

## 🎨 UI Location

```
Main Window (Top Bar)
┌─────────────────────────────────────────────────────┐
│  [Créer facture]              [⚙️ Paramètres]  │ ← Top Right
└─────────────────────────────────────────────────────┘
```

## ⚙️ Settings Dialog Features

- **Browse button**: Select database file via file picker
- **Path display**: Shows selected database path
- **Test connection**: Validates database and shows invoice count
- **Status messages**: Visual feedback on actions
- **Save**: Stores path for future use

## 🔧 Technical Details

### Files Created:
- `Services/ISettingsService.cs` - Settings service interface
- `Services/SettingsService.cs` - Settings service implementation
- `ViewModels/SettingsDialogViewModel.cs` - Settings dialog logic
- `SettingsDialog.axaml` - Settings UI
- `SettingsDialog.axaml.cs` - Settings code-behind
- `DatabaseInitializer.cs` - Database copy logic

### Changes Made:
- `MainWindow.axaml` - Added settings button
- `MainWindowViewModel.cs` - Added OpenSettingsCommand
- `App.axaml.cs` - Copies database on startup
- `ServiceProvider.cs` - (Ready for settings service)

## 📋 Example Paths

Common database locations:
```
C:\ProgramData\Aronium\SimplePos\pos.db
C:\Program Files\YourPOSApp\Data\pos.db
C:\Users\Public\Documents\POSApp\pos.db
```

## 🎯 Next Steps (Optional)

1. **Auto-detect**: Automatically search common locations
2. **Refresh button**: Refresh data without restart
3. **Last sync time**: Show when data was copied
4. **Multiple profiles**: Save multiple database paths

## 💡 Usage Tips

- Configure the path once, then it's saved
- Restart app to get fresh data from main application
- Use when main app is running or closed (safe either way)
- Test connection before saving to verify path is correct

---

**Your settings feature is ready to use! 🎉**















































