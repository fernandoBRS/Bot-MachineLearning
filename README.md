# Bot-MachineLearning
Predicting hybrid car prices integrating Machine Learning and Chat Bots.
The predictive analysis model was created with the **Azure Machine Learning** and the chat bot was created with the **Azure Bot Service** + **Microsoft LUIS**.

# How it works?
Download and install the [Bot Framework Emulator](https://docs.botframework.com/en-us/tools/bot-framework-emulator/).
Run the project and then open the Bot Framework Emulator (remember to configure Local Port, Emulator Url, Bot Url and so on).
In the emulator, type something like *"predict price"*.

The bot will ask you the car model year, acceleration rate, fuel economy (MPG), max value of MPG and MPGe and car model class. 
Then, it will provide the suggested retail price.