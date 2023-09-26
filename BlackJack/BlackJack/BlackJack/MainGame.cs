/* Author: Ali Sher
 * File Name: BlackJack
 * Creation Date: Dec 6th 2016
 * Modification Date: Dec 13th 2016
 * Description: Blackjack game 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BlackJack
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont smallFont;

        //Generic spriteFont
        SpriteFont font;
        SpriteFont largeFont;
        
        //Vector2 for all texts
        Vector2 amountLoc;
        Vector2 betLoc;
        Vector2 walletLoc;

        //Exit screen vector2
        Vector2 mssgLoc;

        //Maintain all the possible game states
        const int PREGAME = 0;
        const int BETTING = 1;
        const int DEAL_CARDS = 2;
        const int PLAYER_TURN = 3;
        const int DEALER_TURN = 4;
        const int POST_GAME = 5;
        const int END_GAME = 6;

        //varaibles for screen size
        int screenWidth;
        int screenLength;

        //CollisionDetection variables
        const bool COLLISION = true;
        const bool NO_COLLISION = false;

        //Store both the current and previous mouse states for user input
        MouseState mouse;
        MouseState prevMouse;

        //The random object allowing for random number generation during Shuffle
        Random rng = new Random();

        //Store all the card related images
        Texture2D deckImg;
        Texture2D faceDownImg;

        //Store where each card in the deck is to be located initially
        Rectangle deckLoc;

        //Store all possible collecions of Cards in the game
        Card[] deck = new Card[52];
        Card[] pHand = new Card[12];
        Card[] dHand = new Card[12];

        //Store a number inidcating where the current top of the deck is, increases as cards are given out
        int topOfDeck = 0;

        //Store the number of cards in each player's hands
        int numPCards = 0;
        int numDCards = 0;

        //Store the current total values of each player's hand
        int pTotal = 0;
        int dTotal = 0;

        //Store the dimensions of a card for alignment purposes
        int cardWidth = 0;
        int cardHeight = 0;

        //Store the current state of the game, initailly in PREGAME
        int gameState = PREGAME;

        //Store the dimensions of the screen, modify these if game resolution is changed
        //float screenWidth = 800;
        //float screenHeight = 480;

        //background variables
        Texture2D bgImg;
        Rectangle bgBox;
        Texture2D bgPlain;
        Rectangle bgPlainBox;

        //button variables
        Texture2D buttonImg;
        Rectangle[] buttonBox;
        const int BTN_INCREASE = 224;

        //timers
        int dealingTimer;
        int exitTimer;
        int dealerCounter;
        int playerCounter;
        int postGameTimer;

        //Betting variables
        int wallet = 200;
        string amountEntered = "";
        int betAmount = 0;
        bool isAmountValid;
        bool canDoubleDown;

        //tryparse bool
        bool wasSuccessful;

        //Array of amount buttons
        Vector2[] numLoc;
        Rectangle[] amountBtnBoxes;

        //Vecotr2 for pregame screen
        Vector2 betBtnLoc;
        Vector2 exitBtnLoc;

        //Vector2 for Betting Screen
        Vector2 enterLoc;
        Vector2 backBtnLoc;
        Vector2 clearBtnLoc;

        //Vector2 for dealer state and player state titles
        Vector2 turnLoc;
        Vector2 playerTitleLoc;
        Vector2 dealerTitleLoc;
        Vector2 instLoc;
        Vector2 DDLoc;

        //Assign keyboardState variable
        KeyboardState keyB;
        KeyboardState prevKeyB;

        //output the result to the user
        int result;
        bool playerWin;
        const int PUSH = 1;
        const int BLACKJACK = 2;
        const int BUST = 3;
        const int GREATER_VALUE = 4;

        //Post game message loc
        Vector2 resultLoc;
        Vector2 countdownLoc;

        //count turns
        int turnCounter;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Set screen size
            graphics.PreferredBackBufferWidth = 1070;
            graphics.PreferredBackBufferHeight = 720;

            //show mouse
            IsMouseVisible = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // set screenwidth and screenlength to the screen
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenLength = graphics.GraphicsDevice.Viewport.Height;

            //initilize rec array for several buttons
            buttonBox = new Rectangle[14];

            //initlize vector2 array
            numLoc = new Vector2[10];

            //initilize for array of amount buttons
            amountBtnBoxes = new Rectangle[10];

            //initilize button tags
            betBtnLoc = new Vector2(255, 305);
            exitBtnLoc = new Vector2(705, 305);

            //betting screen vector2
            enterLoc = new Vector2(650, 80);
            walletLoc = new Vector2(430, 7);
            amountLoc = new Vector2(20, 650);
            betLoc = new Vector2(725, 9);
            backBtnLoc = new Vector2(935, 30);
            clearBtnLoc = new Vector2(1015, 30);

            //exit screen vector2
            mssgLoc = new Vector2(260, 315);

            //Initlize turn vectors
            dealerTitleLoc = new Vector2(55, 0);
            playerTitleLoc = new Vector2(60, 490);
            turnLoc = new Vector2(35, 285);
            instLoc = new Vector2(270, 665);
            DDLoc = new Vector2(560, 665);

            //Post game result loc
            resultLoc = new Vector2(260, 315);
            countdownLoc = new Vector2(0, 680);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            //Load font
            smallFont = Content.Load<SpriteFont>("fonts/smallTest");
            font = Content.Load<SpriteFont>("fonts/Text");
            largeFont = Content.Load<SpriteFont>("fonts/LargeFont");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //Load all card images
            deckImg = Content.Load<Texture2D>("images/sprites/CardFaces");
            faceDownImg = Content.Load<Texture2D>("images/sprites/CardBack");

            //Calculate and store the dimensions of a card
            cardWidth = deckImg.Width / Card.CARDS_IN_SUIT;
            cardHeight = deckImg.Height / Card.NUM_SUITS;

            //NOTE: You can move this if you would like
            deckLoc = new Rectangle((int)screenWidth - 150, 60, cardWidth, cardHeight);

            //Create the initial deck
            CreateDeck();

            //Set background
            bgImg = Content.Load<Texture2D>("images/backgrounds/GreenFelt(good)");
            bgBox = new Rectangle(0, 0, screenWidth, screenLength);

            bgPlain = Content.Load<Texture2D>("images/backgrounds/GreenFelt");
            bgPlainBox = new Rectangle(0, 0, screenWidth, screenLength);

            //load buttons
            buttonImg = Content.Load<Texture2D>("images/sprites/button");

            //PREGAME BUTTONS
            //Betting Button
            buttonBox[0] = new Rectangle(200, 260, 200, 200);
            //Exit Button
            buttonBox[1] = new Rectangle(650, 260, 200, 200);

            //BETTING BUTTONS
            //Clear Button
            buttonBox[2] = new Rectangle(995, 1, 75, 75);
            //Back Button
            buttonBox[3] = new Rectangle(915, 1, 75, 75);

            //Load content and set location of amount buttons

            numLoc[1] = new Vector2(65, 275);
            amountBtnBoxes[1] = new Rectangle(35, 260, 75, 75);
            numLoc[2] = new Vector2(150, 275);
            amountBtnBoxes[2] = new Rectangle(120, 260, 75, 75);
            numLoc[3] = new Vector2(235, 275);
            amountBtnBoxes[3] = new Rectangle(205, 260, 75, 75);
            numLoc[4] = new Vector2(325, 275);
            amountBtnBoxes[4] = new Rectangle(290, 260, 75, 75);
            numLoc[5] = new Vector2(405, 275);
            amountBtnBoxes[5] = new Rectangle(375, 260, 75, 75);
            numLoc[6] = new Vector2(490, 275);
            amountBtnBoxes[6] = new Rectangle(460, 260, 75, 75);
            numLoc[7] = new Vector2(575, 275);
            amountBtnBoxes[7] = new Rectangle(545, 260, 75, 75);
            numLoc[8] = new Vector2(660, 275);
            amountBtnBoxes[8] = new Rectangle(630, 260, 75, 75);
            numLoc[9] = new Vector2(745, 275);
            amountBtnBoxes[9] = new Rectangle(715, 260, 75, 75);
            numLoc[0] = new Vector2(830, 275);
            amountBtnBoxes[0] = new Rectangle(800, 260, 75, 75);

            //Shuffle the deck before going into PREGAME, this will need to be done each time
            //PREGAME is entered
            ShuffleDeck(1000);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                gameState = END_GAME;
            }

            // keyboard state
            prevKeyB = keyB;
            keyB = Keyboard.GetState();
            

            // TODO: Add your update logic here
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //Update the current gameState logic only
            switch (gameState)
            {
                case PREGAME:
                    
                    //effect that alerts the user to mousing over the button
                    if (MouseClicked(buttonBox[0]))
                    {
                        buttonBox[0].X = 188;
                        buttonBox[0].Y = 248;
                        buttonBox[0].Width = BTN_INCREASE;
                        buttonBox[0].Height = BTN_INCREASE;
                    }
                    else
                    {
                        buttonBox[0].X = 200;
                        buttonBox[0].Y = 260;
                        buttonBox[0].Width = 200;
                        buttonBox[0].Height = 200;
                    }

                    if (MouseClicked(buttonBox[1]))
                    {
                        buttonBox[1].X = 638;
                        buttonBox[1].Y = 248;
                        buttonBox[1].Width = BTN_INCREASE;
                        buttonBox[1].Height = BTN_INCREASE;
                    }
                    else
                    {
                        buttonBox[1].X = 650;
                        buttonBox[1].Y = 260;
                        buttonBox[1].Width = 200;
                        buttonBox[1].Height = 200;
                    }

                    //Go to Betting
                    if (NewMouseClick() && MouseClicked(buttonBox[0]))
                    {
                        gameState = BETTING;
                    }

                    //Exit
                    if (NewMouseClick() && MouseClicked(buttonBox[1]))
                    {
                        gameState = END_GAME;
                    }

                    break;
                case BETTING:

                    //let user see amount they are entering
                    if (NewMouseClick() && MouseClicked(amountBtnBoxes[1]))
                    {
                        amountEntered = amountEntered + "1";
                    }
                    if (NewMouseClick() && MouseClicked(amountBtnBoxes[2]))
                    {
                        amountEntered = amountEntered + "2";
                    }
                    if (NewMouseClick() && MouseClicked(amountBtnBoxes[3]))
                    {
                        amountEntered = amountEntered + "3";
                    }
                    if (NewMouseClick() && MouseClicked(amountBtnBoxes[4]))
                    {
                        amountEntered = amountEntered + "4";
                    }
                    if (NewMouseClick() && MouseClicked(amountBtnBoxes[5]))
                    {
                        amountEntered = amountEntered + "5";
                    }
                    if (NewMouseClick() && MouseClicked(amountBtnBoxes[6]))
                    {
                        amountEntered = amountEntered + "6";
                    }
                    if (NewMouseClick() && MouseClicked(amountBtnBoxes[7]))
                    {
                        amountEntered = amountEntered + "7";
                    }
                    if (NewMouseClick() && MouseClicked(amountBtnBoxes[8]))
                    {
                        amountEntered = amountEntered + "8";
                    }
                    if (NewMouseClick() && MouseClicked(amountBtnBoxes[9]))
                    {
                        amountEntered = amountEntered + "9";
                    }
                    if (NewMouseClick() && MouseClicked(amountBtnBoxes[0]))
                    {
                        amountEntered = amountEntered + "0";
                    }

                    //if amount is too long
                    if (amountEntered.Length >= 7)
                    {
                        amountEntered = "";
                    }

                    //if player wants to clear
                    if (NewMouseClick() && MouseClicked(buttonBox[2]))
                    {
                        amountEntered = "";
                    }

                    //convert amount entered into bet amount
                    wasSuccessful = Int32.TryParse(amountEntered, out betAmount);

                    //All logic for validating bet amount
                    if (betAmount < wallet && betAmount > 0)
                    {
                        isAmountValid = true;
                    }
                    else
                    {
                        isAmountValid = false;
                    }

                    //Allow player to continue
                    if (isAmountValid == true && keyB.IsKeyDown(Keys.Enter))
                    {
                        gameState = DEAL_CARDS;
                    }

                    //Player can quit
                    if (NewMouseClick() && MouseClicked(buttonBox[3]))
                    {
                        gameState = PREGAME;
                    }

                    break;
                case DEAL_CARDS:

                    dealingTimer++;

                    //Deal player cards

                    if (dealingTimer == 60)
                    {
                        DealCards(pHand, dHand, deck);
                    }

                    //Switch to player turn
                    if (dealingTimer == 300)
                    {
                        gameState = PLAYER_TURN;
                    }

                    break;
                case PLAYER_TURN:

                    //Stand
                    if (keyB.IsKeyDown(Keys.S) && prevKeyB.IsKeyUp(Keys.S))
                    {
                        gameState = DEALER_TURN;
                    }

                    //Hit
                    if (keyB.IsKeyDown(Keys.H) && prevKeyB.IsKeyUp(Keys.H))
                    {
                        HitCardP();
                        //Also be able to draw the new card
                        pHand[numPCards - 1].dest.X = pHand[numPCards - 2].dest.X + 45;
                        pHand[numPCards - 1].dest.Y = pHand[numPCards - 2].dest.Y;
                        gameState = DEALER_TURN;

                    }

                    if (betAmount * 2 < wallet)
                    {
                        canDoubleDown = true;
                    }
                    else
                    {
                        canDoubleDown = false;
                    }

                    //DD
                    if (keyB.IsKeyDown(Keys.D) && prevKeyB.IsKeyUp(Keys.D) && canDoubleDown == true)
                    {
                        betAmount = betAmount * 2;
                        gameState = DEALER_TURN;
                    }

                    //Calculate total for the dealer and for the player
                    pTotal = CheckTotal(pHand, numPCards);
                    dTotal = CheckTotal(dHand, numDCards);

                    if (pTotal >= 21)
                    {
                        playerCounter++;
                        if (playerCounter == 60)
                        {
                            gameState = POST_GAME;
                        }
                    }
                     break;

                case DEALER_TURN:

                     pTotal = CheckTotal(pHand, numPCards);
                     dTotal = CheckTotal(dHand, numDCards);

                     if (dTotal < 17)
                     {
                         dealerCounter++;

                         if (dealerCounter == 60)
                         {
                             HitCardD();
                             //Also be able to draw the new card
                             dHand[numDCards - 1].dest.X = dHand[numDCards - 2].dest.X + 45;
                             dHand[numDCards - 1].dest.Y = dHand[numDCards - 2].dest.Y;
                             dealerCounter = 0;
                         }
                     }

                     if (dTotal >= 17)
                     {
                         dealerCounter++;

                         if (dealerCounter == 60)
                         {
                             gameState = PLAYER_TURN;
                             dealerCounter = 0;
                         }
                     }

                     if (dTotal >= 21)
                     {
                         dealerCounter++;

                         if (dealerCounter == 60)
                         {
                             gameState = POST_GAME;
                         }
                     }

                     if (dealerCounter == 60)
                     {
                         turnCounter = 5;
                         gameState = PLAYER_TURN;
                     }

                    if (turnCounter == 5)
                    {
                        gameState = POST_GAME;
                    }

                    break;
                case POST_GAME:

                    //Bust
                    if (pTotal > 21)
                    {
                        result = BUST;
                        playerWin = false;

                        wallet -= betAmount;
                    }

                    if (dTotal > 21)
                    {
                        result = BUST;
                        playerWin = true;

                        wallet += betAmount;
                    }

                    //Push
                    if (pTotal == 21 && dTotal == 21)
                    {
                        result = PUSH;

                    }

                    if (pTotal == dTotal)
                    {
                        result = PUSH;

                    }

                    //Blackjack
                    if (pTotal == 21 && dTotal < 21 && numPCards == 2)
                    {
                        result = BLACKJACK;
                        playerWin = true;

                        wallet += (betAmount * (int)1.5);
                    }

                    if (dTotal == 21 && pTotal < 21 && numDCards == 2)
                    {
                        result = BLACKJACK;
                        playerWin = false;
                         
                        wallet -= betAmount;
                    }

                    //Greater Amount
                    if (pTotal > dTotal && pTotal < 21)
                    {
                        result = GREATER_VALUE;
                        playerWin = true;

                        wallet += betAmount;
                    }

                    if (dTotal > pTotal && dTotal < 21)
                    {
                        result = GREATER_VALUE;
                        playerWin = false;

                        wallet -= betAmount;
                    }

                    postGameTimer++;

                    //autimatically sends player to pregame screen
                    if (postGameTimer == 600)
                    {
                        gameState = PREGAME;
                    }

                    break;
                case END_GAME:

                    exitTimer++;

                    if (exitTimer == 180)
                    {
                        this.Exit();
                    }

                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //draw background
            spriteBatch.Draw(bgPlain, bgPlainBox, Color.White);

            //Draw the current gameState graphics only
            switch (gameState)
            {
                case PREGAME:
                //draw bacground
                    spriteBatch.Draw(bgImg, bgBox, Color.White);
                //draw buttons

                    //Betting Button
                    spriteBatch.Draw(buttonImg, buttonBox[0], Color.White);
                    spriteBatch.DrawString(font, "Bet", betBtnLoc, Color.Black);

                    //Exit Button
                    spriteBatch.Draw(buttonImg, buttonBox[1], Color.White);
                    spriteBatch.DrawString(font, "Exit", exitBtnLoc, Color.Black);
                    
                    break;
                case BETTING:

                    //Draw buttons for the betting
                    //Clear button
                    spriteBatch.Draw(buttonImg, buttonBox[2], Color.White);
                    spriteBatch.DrawString(smallFont, "Clear", clearBtnLoc, Color.Black);
                    //Back button
                    spriteBatch.Draw(buttonImg, buttonBox[3], Color.White);
                    spriteBatch.DrawString(smallFont, "Back", backBtnLoc, Color.Black);
                    

                    //Automatically draw all 10 buttons and their numbers
                    for (int i = 0; i <= 9; i++)
                    {
                        spriteBatch.Draw(buttonImg, amountBtnBoxes[i], Color.White);
                        spriteBatch.DrawString(font, "" + i, numLoc[i], Color.Blue);
                    }


                        //user feedback if amount valid is false
                        if (isAmountValid == false)
                        {
                            spriteBatch.DrawString(largeFont, "$" + amountEntered, amountLoc, Color.Red);
                            //enter to continue
                            spriteBatch.DrawString(font, "Press Enter to Continue", enterLoc, Color.Red);
                        }

                        if (isAmountValid == true)
                        {
                            spriteBatch.DrawString(largeFont, "$" + amountEntered, amountLoc, Color.Blue);
                            //enter to continue
                            spriteBatch.DrawString(font, "Press Enter to Continue", enterLoc, Color.Blue);
                        }

                        //wallet amount
                        spriteBatch.DrawString(font, "Wallet: $" + wallet, walletLoc, Color.Blue);

                    break;
                case DEAL_CARDS:

                    //Draw the cards like a stack
                    for (int i = 0; i <= 51; i++)
                    {
                    deck[i].dest.X = 950 + (1 * -i);
                    deck[i].dest.Y = 25 + (1 * i);
                    deck[i].Draw(spriteBatch);
                    }

                    //Location of player cards
                    if (dealingTimer >= 60)
                    {
                        pHand[0].dest.X = 70;
                        pHand[0].dest.Y = 550;
                        pHand[1].dest.X = 115;
                        pHand[1].dest.Y = 550;
                    }

                    //Location of dealer cards
                    if (dealingTimer >= 180)
                    {
                        dHand[0].dest.X = 70;
                        dHand[0].dest.Y = 50;
                        dHand[1].dest.X = 115;
                        dHand[1].dest.Y = 50;
                    }

                    if (dealingTimer >= 60)
                    {
                       pHand[0].Draw(spriteBatch);
                    }

                    if (dealingTimer >= 120)
                    {
                        pHand[1].Draw(spriteBatch);
                    }

                    if (dealingTimer >= 180)
                    {
                       dHand[0].Draw(spriteBatch);
                    }

                    if (dealingTimer >= 240)
                    {
                       dHand[1].Draw(spriteBatch);
                    }
                        
                    //Write titles for whose cards are whose
                    spriteBatch.DrawString(font, "Player", playerTitleLoc , Color.Blue);
                    spriteBatch.DrawString(font, "Dealer" , dealerTitleLoc, Color.Blue);

                    break;
                case PLAYER_TURN:

                    //Update to draw in case new cards are added
                    for (int i = 0; i < numPCards; i++)
                    {
                        pHand[i].Draw(spriteBatch);
                    }

                    //Update to draw in case new cards are added
                    for (int i = 0; i < numDCards; i++)
                    {
                        dHand[i].Draw(spriteBatch);
                    }

                    //Write titles for whose cards are whose
                    spriteBatch.DrawString(font, "Player            " + pTotal, playerTitleLoc, Color.Blue);
                    spriteBatch.DrawString(font, "Dealer            " + dTotal, dealerTitleLoc, Color.Blue);
                    spriteBatch.DrawString(largeFont, "Player Turn", turnLoc, Color.Black);

                    //Draw instruvtions for player
                    spriteBatch.DrawString(font, "H: Hit S: Stand", instLoc, Color.Blue);

                    if (canDoubleDown == false)
                    {
                        spriteBatch.DrawString(font, "D: Double Down", DDLoc, Color.Red);
                    }
                    
                    if (canDoubleDown == true)
                    {
                        spriteBatch.DrawString(font, "D: Double Down", DDLoc, Color.Blue);
                    }

                    //currency amounts
                    spriteBatch.DrawString(font, "Wallet: $" + wallet, walletLoc, Color.Blue);
                    spriteBatch.DrawString(font, "Bet: $" + betAmount, betLoc, Color.Blue);

                    break;
                case DEALER_TURN:

                    //Draw pCards from deal
                    for (int i = 0; i < numPCards; i++)
                    {
                        pHand[i].Draw(spriteBatch);
                    }

                    //Draw dCards from deal
                    for (int i = 0; i < numDCards; i++)
                    {
                        dHand[i].Draw(spriteBatch);
                    }

                    //Write titles for whose cards are whose
                    spriteBatch.DrawString(font, "Player            " + pTotal, playerTitleLoc, Color.Blue);
                    spriteBatch.DrawString(font, "Dealer            " + dTotal, dealerTitleLoc, Color.Blue);
                    spriteBatch.DrawString(largeFont, "Dealer Turn", turnLoc, Color.Black);

                    //Draw instruvtions for player
                    spriteBatch.DrawString(font, "H: Hit S: Stand D: Double Down", instLoc, Color.Red);

                    break;
                case POST_GAME:

                    //show wallet
                    spriteBatch.DrawString(font, "Wallet: $" + wallet, walletLoc, Color.Blue);

                    //Show player and dealer scores
                    spriteBatch.DrawString(font, "Dealer Score: " + dTotal, dealerTitleLoc, Color.Black);
                    spriteBatch.DrawString(font, "Player Score" + pTotal, playerTitleLoc, Color.Black);


                    //show the result to the user
                    switch (result)
                    {
                        case BUST:

                            if (playerWin == true)
                            {
                                spriteBatch.DrawString(font, "Congratulations! Dealer got a bust and you won!" , turnLoc, Color.Black);
                            }
                            if (playerWin == false)
                            {
                                spriteBatch.DrawString(font, "Play Again! You got a bust and the dealer won!", turnLoc, Color.Black);
                            }
                            break;

                        case PUSH:

                            spriteBatch.DrawString(font, "Push! Play again!", turnLoc, Color.Black);
                            break;

                        case BLACKJACK:

                            if (playerWin == true)
                            {
                                spriteBatch.DrawString(font, "Congratulations! You got a Blackjack and won!", turnLoc, Color.Black);
                            }
                            if (playerWin == false)
                            {
                                spriteBatch.DrawString(font, "Play Again! Dealer got a Blackjack and won!", turnLoc, Color.Black);
                            }
                            break;

                        case GREATER_VALUE:

                             if (playerWin == true)
                            {
                                spriteBatch.DrawString(font, "Congratulations! You have a greater score and win", turnLoc, Color.Black);
                            }
                            if (playerWin == false)
                            {
                                spriteBatch.DrawString(font, "Play Again! Dealer has a great score and wins!", turnLoc, Color.Black);
                            }
                            break;
                    }

                    //show the countdown to the user to return to the menu

                        if (postGameTimer > 0 && postGameTimer < 60)
                        {
                            spriteBatch.DrawString(font, "Return to the menu in: 10", countdownLoc, Color.Black);
                        }

                        if (postGameTimer > 60 && postGameTimer < 120)
                        {
                            spriteBatch.DrawString(font, "Return to the menu in: 9", countdownLoc, Color.Black);
                        }

                        if (postGameTimer > 120 && postGameTimer < 180)
                        {
                            spriteBatch.DrawString(font, "Return to the menu in: 8", countdownLoc, Color.Black);

                        }

                        if (postGameTimer > 180 && postGameTimer < 240)
                        {
                            spriteBatch.DrawString(font, "Return to the menu in: 7", countdownLoc, Color.Black);
                        }

                        if (postGameTimer > 240 && postGameTimer < 300)
                        {
                            spriteBatch.DrawString(font, "Return to the menu in: 6", countdownLoc, Color.Black);
                        }

                        if (postGameTimer > 300 && postGameTimer < 360)
                        {
                            spriteBatch.DrawString(font, "Return to the menu in: 5", countdownLoc, Color.Black);
                        }

                        if (postGameTimer > 360 && postGameTimer < 420)
                        {
                            spriteBatch.DrawString(font, "Return to the menu in: 4", countdownLoc, Color.Black);
                        }

                        if (postGameTimer > 420 && postGameTimer < 480)
                        {
                            spriteBatch.DrawString(font, "Return to the menu in: 3", countdownLoc, Color.Black);
                        }

                        if (postGameTimer > 480 && postGameTimer < 540)
                        {
                            spriteBatch.DrawString(font, "Return to the menu in: 2", countdownLoc, Color.Black);
                        }

                        if (postGameTimer > 540 && postGameTimer < 600)
                        {
                            spriteBatch.DrawString(font, "Return to the menu in: 1", countdownLoc, Color.Black);
                        }


                    break;
                case END_GAME:
                    spriteBatch.Draw(bgImg, bgBox, Color.White);
                    spriteBatch.DrawString(largeFont, "Thanks for Playing! \n You're leaving with $" + wallet + ".", mssgLoc, Color.Blue);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Creates each of the 52 cards in a standard deck of cards and adds them to the deck array
        /// </summary>
        private void CreateDeck()
        {
            //count tracks the number of cards created
            int count = 0;

            //For every suit, create each card from Ace to King
            for (int i = 0; i < Card.NUM_SUITS; i++)
            {
                for (int j = 0; j < Card.CARDS_IN_SUIT; j++)
                {
                    //Create and add the new card to the deck array
                    deck[count] = new Card(deckImg, faceDownImg, deckLoc, i, j);
                    count++;
                }
            }
        }

        //Pre:
        //Post:
        //Desc:
        private void ShuffleDeck(int numShuffles)
        {
            //TODO: loop numShuffles times and generate 2 random numbers from 0 and deck.Length.  Swap the elements
            //      in deck at those elements.
            //      This may also be a good place to reset any individual round data, e.g. bet, numDCards, numPCards, etc.
            
            for (int i = 0; i <= numShuffles; i++)
            {
                int num1 = rng.Next(0, 52);
                int num2 = rng.Next(0, 52);
                int shuffleHolder = 0;
                deck[shuffleHolder] = deck[num1];
                deck[num1] = deck[num2];
                deck[num2] = deck[shuffleHolder];
            }
        }

        //Pre:
        //Post:
        //Desc:
        private int GetHandTotal(Card[] hand, int numCardsInHand)
        {
            //int aceCount = 0;
            int total = 0;

            //TODO: Calculate the total value of the Cards in the hand array

            return total;
        }

        //Pre:
        //Post:
        //Desc:
        private bool NewMouseClick()
        {
            Boolean result = false;

            //TODO: Detect whether a new mouse button click occured

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                result = true;
            }

            return result;
        }

        //Pre:
        //Post:
        //Desc:
        private bool MouseClicked(Rectangle box)
        {
            Boolean result = false;

            //TODO: Detect whether mouse.X and mouse.Y both are inside the box parameter
            if (box.Contains(mouse.X, mouse.Y))
            {
                result = true;
            }

            return result;
        }

        //Collision Detection Subprogram
        private bool CollisionDetection(Rectangle box1, Rectangle box2)
        {
            if (!(
                box1.Bottom < box2.Top ||
                box1.Right < box2.Left ||
                box1.Top > box2.Bottom ||
                box1.Left > box2.Right))
            {
                return COLLISION;
            }

                return NO_COLLISION;
        }

        private int CheckTotal(Card[] hand, int numCards)
        {
            int result = 0;
            int aceCounter = 0;

            for (int i = 0; i < numCards; i++)
            {

                //if (hand[i].isFaceUp == true)

                {
                    if (hand[i].symbol == Card.TWO)
                    {
                        result += 2;
                    }

                    if (hand[i].symbol == Card.THREE)
                    {
                        result += 3;
                    }

                    if (hand[i].symbol == Card.FOUR)
                    {
                        result += 4;
                    }

                    if (hand[i].symbol == Card.FIVE)
                    {
                        result += 5;
                    }

                    if (hand[i].symbol == Card.SIX)
                    {
                        result += 6;
                    }

                    if (hand[i].symbol == Card.SEVEN)
                    {
                        result += 7;
                    }

                    if (hand[i].symbol == Card.EIGHT)
                    {
                        result += 8;
                    }

                    if (hand[i].symbol == Card.NINE)
                    {
                        result += 9;
                    }

                    if (hand[i].symbol == Card.TEN)
                    {
                        result += 10;
                    }

                    if (hand[i].symbol == Card.KING)
                    {
                        result += 10;
                    }

                    if (hand[i].symbol == Card.QUEEN)
                    {
                        result += 10;
                    }

                    if (hand[i].symbol == Card.JACK)
                    {
                        result += 10;
                    }

                    if (hand[i].symbol == Card.ACE)
                    {
                        aceCounter += 1;
                    }
                }
            }

            result += (aceCounter * 11);

            if (result > 21)
            {
                result -= (aceCounter * 11);
                result += (aceCounter * 1);
            }

            return result;
        }

        private void DealCards(Card[] player, Card[] dealer, Card[] deck)
        {
            //give card to player
            player[numPCards] = deck[topOfDeck];
            player[numPCards].isFaceUp = true;
            topOfDeck++;
            numPCards++;

            player[numPCards] = deck[topOfDeck];
            player[numPCards].isFaceUp = true;
            topOfDeck++;
            numPCards++;


            //give card to dealer
            dealer[numDCards] = deck[topOfDeck];
            topOfDeck++;
            numDCards++;

            dealer[numDCards] = deck[topOfDeck];
            dealer[numDCards].isFaceUp = true;
            topOfDeck++;
            numDCards++;

        }

        //Subprogram player hit
        private void HitCardP()
        {
            //give card to whoever hit
            pHand[numPCards] = deck[topOfDeck];
            pHand[numPCards].isFaceUp = true;
            topOfDeck++;
            numPCards++;
        }

        //Subprogram if dealer hit
        private void HitCardD()
        {
            //give card to whoever hit
            dHand[numDCards] = deck[topOfDeck];
            dHand[numDCards].isFaceUp = true;
            topOfDeck++;
            numDCards++;
        }
    }
}
