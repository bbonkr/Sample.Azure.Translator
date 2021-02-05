# Sample codes for Azure Translator

[Azure Translator](https://azure.microsoft.com/services/cognitive-services/translator/) is an AI service for real-time text translation.

## Requirements

1. Make sure to create Azure Translator resource.
2. Make sure to have access your azure translator subscription key.
   Go to your azure translator resource then click key and endpoint menu.

## Configure

Edit src\Sample.Azure.Translator.App\appsettings.json file.

```json
{
  "Translator": {
    "Endpoint": "endpoint here",
    "SubscriptionKey": "subscription key here",
    "Region": "region here"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

## Run

Sample app translate input text or file to en, ru, ja.

### Use text file

```bash
$ cd src/Sample.Azure.Translator.App
$ dotnet run -- ~/sample.txt
Result: [
  {
    "DetectedLanguage": {
      "Language": "ko",
      "Score": 0.96
    },
    "SourceText": null,
    "Translations": [
      {
        "Text": "If you are using multiple service resources in the Negative Service. This allows you to authenticate requests for multiple services using a single secret key. ",
        "Transliteration": null,
        "To": "en",
        "Alignment": null,
        "SentLen": null
      },
      {
        "Text": "Если вы используете несколько ресурсов службы в отрицательной службе. Это позволяет проверить подлинность запросов на несколько служб с помощью одного секретного ключа. ",
        "Transliteration": null,
        "To": "ru",
        "Alignment": null,
        "SentLen": null
      },
      {
        "Text": "ネガティブ サ?ビスで複?のサ?ビス リソ?スを使用している場合。これにより、?一の秘密キ?を使用して複?のサ? ビスの要求を認?できます。",
        "Transliteration": null,
        "To": "ja",
        "Alignment": null,
        "SentLen": null
      }
    ]
  }
]
```

### Use console input

```bash
$ cd src/Sample.Azure.Translator.App
Please type to want translation then press a return key (or enter key).
If want to exit this app, press 'ctrl' + 'c'.
Input text:
안녕하세요. 고마워요.
info: Sample.Azure.Translator.App.Services.TranslatorService[0]
      Request uri: https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to=en&to=ru&to=ja
Result: [
  {
    "DetectedLanguage": {
      "Language": "ko",
      "Score": 1
    },
    "SourceText": null,
    "Translations": [
      {
        "Text": "Hello. Thank you.",
        "Transliteration": null,
        "To": "en",
        "Alignment": null,
        "SentLen": null
      },
      {
        "Text": "Привет. Спасибо.",
        "Transliteration": null,
        "To": "ru",
        "Alignment": null,
        "SentLen": null
      },
      {
        "Text": "こんにちは。ありがとうございました。",
        "Transliteration": null,
        "To": "ja",
        "Alignment": null,
        "SentLen": null
      }
    ]
  }
]
```
