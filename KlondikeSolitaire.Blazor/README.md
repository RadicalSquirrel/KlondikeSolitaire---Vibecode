# Klondike Solitaire - Blazor WebAssembly

A modern, interactive web-based implementation of Klondike Solitaire built with Blazor WebAssembly.

## Features

- **Interactive Gameplay**: Click-based card movement with visual feedback
- **Customizable Rules**:
  - Draw 1 or 3 cards from stock
  - King-only or any card in empty tableau
  - Optional Foundation→Tableau moves
  - Configurable stock redeal limits
- **Scoring System**: Track points and time
- **Statistics Tracking**: Persistent game statistics stored in browser localStorage
- **Responsive Design**: Beautiful card animations and green felt table aesthetic
- **Undo Functionality**: Unlimited undo of moves
- **Win Detection**: Automatic celebration when you win!

## Running the Application

### Development Mode

```bash
cd KlondikeSolitaire.Blazor
dotnet run
```

Then open your browser to `https://localhost:5001` (or the URL shown in the terminal).

### Build for Production

```bash
dotnet publish -c Release
```

The published files will be in `bin/Release/net8.0/publish/wwwroot/` and can be hosted on any static web server.

## Project Structure

```
KlondikeSolitaire.Blazor/
├── Components/
│   ├── CardComponent.razor       # Individual card display
│   ├── GameBoard.razor           # Main game board with tableau, foundations, etc.
│   └── GameSettings.razor        # New game configuration dialog
├── Pages/
│   ├── Home.razor                # Main game page
│   └── Statistics.razor          # Statistics display page
├── Services/
│   ├── GameService.cs            # Game state management
│   └── StatisticsService.cs     # Statistics persistence (localStorage)
└── wwwroot/
    └── css/
        └── game.css              # Game-specific styles
```

## How to Play

1. **Start a New Game**: Click "New Game" and configure your preferred rules
2. **Draw Cards**: Click the stock pile to draw cards
3. **Move Cards**: Click a card to select it, then click the destination
4. **Build Foundations**: Move Aces to foundations and build up by suit
5. **Build Tableau**: Alternate red/black cards in descending order
6. **Undo Moves**: Click "Undo" to reverse your last move
7. **Win**: Get all 52 cards into the four foundation piles!

## Technologies Used

- **Blazor WebAssembly** (.NET 8)
- **KlondikeSolitaire.Core** - Shared game logic library
- **Browser LocalStorage** - For statistics persistence
- **CSS3** - Animations and styling

## Progressive Web App (PWA)

This application is a full PWA and can be installed on your device for native-like experience:

### Installing on iPhone/iPad

1. Open the app in Safari
2. Tap the Share button (square with arrow pointing up)
3. Scroll down and tap "Add to Home Screen"
4. Tap "Add" to install
5. The app will appear on your home screen with its icon
6. Launch it for a full-screen, native-like experience

### Installing on Android

1. Open the app in Chrome
2. Tap the three-dot menu
3. Select "Install app" or "Add to Home Screen"
4. Follow the prompts to install
5. Launch from your app drawer

### Installing on Desktop

1. Open the app in Chrome, Edge, or other Chromium browser
2. Look for the install icon in the address bar (computer with down arrow)
3. Click it and follow the prompts
4. The app will be available as a standalone application

### PWA Features

- **Offline Support**: Service worker caches resources for offline play
- **Installable**: Add to home screen for native app experience
- **Standalone Mode**: Runs fullscreen without browser chrome
- **App Icons**: Custom icons for all platforms
- **No Zoom**: Optimized viewport prevents accidental zooming
- **Auto Updates**: Automatically updates when new version is deployed

## Browser Compatibility

Works in all modern browsers that support WebAssembly:
- Chrome/Edge 91+
- Firefox 89+
- Safari 15+ (iOS 15+ for PWA features)
