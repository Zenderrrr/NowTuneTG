# NowTuneTG

Sync your Spotify “Now Playing” track to your Telegram bio in real time.

---

## Features

- 🎵 Reads your current Spotify track
- 🔄 Automatically updates your Telegram bio
- ⚡ Updates only when the track changes (no unnecessary API calls)
- 🔐 Secure token storage (no secrets in repository)
- ♻️ Uses refresh tokens (no repeated login required)
- ⏱ Runs continuously with configurable interval

---

## Tech Stack

- **C# / .NET 8**
- **Spotify Web API** (via SpotifyAPI.Web)
- **Telegram MTProto API** (via WTelegramClient)

---

## How it works

1. Authenticates with Spotify (OAuth Authorization Code Flow)
2. Retrieves the currently playing track
3. Formats it into a short string
4. Updates your Telegram bio using MTProto
5. Repeats every ~20–30 seconds

Spotify → BioFormatter → SyncService → Telegram bio


---

## Setup

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/NowTuneTG.git
cd NowTuneTG
```

Create configuration file

Create:

```appsettings.json```

### 2. Create configuration file

Based on:

```appsettings.example.json```

Example:

```JSON
{
  "Spotify": {
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET",
    "RedirectUri": "http://127.0.0.1:5000/callback/"
  },
  "Telegram": {
    "ApiId": 123456,
    "ApiHash": "YOUR_API_HASH"
  }
}
```

### 3. Spotify setup

1. Go to Spotify Developer Dashboard
2. Create a new application
3. Add redirect URI: ```http://127.0.0.1:5000/callback/```

4. Copy:

- Client ID
- Client Secret

### 4. Telegram setup
1. Go to ```https://my.telegram.org```
2. Login and open API development tools
3. Create an application
4. Copy:

- api_id
- api_hash

### 5. Run the application

```Bash 
dotnet run
```
First run:

- Open Spotify login URL
- Paste returned code
- Enter Telegram phone number
- Enter Telegram verification code

After first run:

- Tokens and sessions are saved locally
- No need to login again

## Project Structure

Models/
  NowPlaying.cs
  SpotifySettings.cs
  SpotifyTokenInfo.cs
  TelegramSettings.cs

Services/
  SpotifyService.cs
  SpotifyTokenStore.cs
  TelegramProfileService.cs
  SyncService.cs
  BioFormatter.cs

Program.cs


## Security

The following files are excluded from version control:

- appsettings.json
- spotify_tokens.json
- *.session

⚠️ Never commit secrets.

## Notes

- Telegram bio length is limited (~70 characters)
- Updates are skipped if track hasn't changed
- App must stay running to keep syncing
- Works best when system sleep is disabled

## Build (optional)

Publish executable:

```Bash
dotnet publish -c Release -r win-x64 --self-contained true
```

## License

MIT