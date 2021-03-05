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

### Arguments

- `--to=<string>`: (required) Languages to translate. Comma separated string. e.g.) --to=en,ko,ja
- `--from=<string>`: (optional) Input text language.
- `--html`: (optional) Input text is html.
- `--each-language`: (optional) Request to translate each language

### Use text file

```bash
$ cd src/Sample.Azure.Translator.App
$ dotnet run -- ../../samples/sample.txt --from=ko --to=en,ja,es
Result: {
  "Data": [
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
          "Text": "ネガティブ サ?ビスで複?のサ?ビス リソ?スを使用している場合。これにより、?一の秘密キ?を使用して複?のサ?ビスの要求を認?できます。",
          "Transliteration": null,
          "To": "ja",
          "Alignment": null,
          "SentLen": null
        },
        {
          "Text": "Если вы используете несколько ресурсов службы в отрицательной службе. Это позволяет проверить подлинность запросов на несколько служб с помощью одного секретного ключа. ",
          "Transliteration": null,
          "To": "ru",
          "Alignment": null,
          "SentLen": null
        }
      ]
    }
  ],
  "StatusCode": 200,
  "Message": ""
}
```

### Use console input

```bash
$ cd src/Sample.Azure.Translator.App
$ dotnet run -- --to=en,ja,ru
Please type to want translation then press a return key (or enter key).
If want to exit this app, press 'ctrl' + 'c'.
Input text:
안녕하세요. 고마워요.
Result: {
  "Data": [
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
          "Text": "こんにちは。ありがとうございました。",
          "Transliteration": null,
          "To": "ja",
          "Alignment": null,
          "SentLen": null
        },
        {
          "Text": "Привет. Спасибо.",
          "Transliteration": null,
          "To": "ru",
          "Alignment": null,
          "SentLen": null
        }
      ]
    }
  ],
  "StatusCode": 200,
  "Message": ""
}
```

### Use html file


```bash
$ cd src/Sample.Azure.Translator.App
$ dotnet run -- --html --to=ko ../../samples/sample.html
Result: {
  "Data": [
    {
      "DetectedLanguage": {
        "Language": "ko",
        "Score": 0.95
      },
      "SourceText": null,
      "Translations": [
        {
          "Text": "<div class=\"blog-post-content\">\r\n    <p>Look back to 2020.</p>\r\n    <h2 id=\"기술-스택\" style=\"position:relative;\"><a href=\"#%EA%B8%B0%EC%88%A0-%EC%8A%A4%ED%83%9D\" aria-label=\"기술 스택 permalink\" class=\"anchor before\"><svg aria-hidden=\"true\" focusable=\"false\" height=\"16\" version=\"1.1\" viewbox=\"0 0 16 16\" width=\"16\">\r\n                <path fill-rule=\"evenodd\" d=\"M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z\">\r\n                </path>\r\n            </svg></a>Technology Stack</h2>\r\n    <h3 id=\"net-5\" style=\"position:relative;\"><a href=\"#net-5\" aria-label=\"net 5 permalink\" class=\"anchor before\"><svg aria-hidden=\"true\" focusable=\"false\" height=\"16\" version=\"1.1\" viewbox=\"0 0 16 16\" width=\"16\">\r\n
     <path fill-rule=\"evenodd\" d=\"M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z\">\r\n                </path>\r\n            </svg></a>.NET 5</h3>\r\n    <p>You are transferring your previous .NET Core 2.x to 3.x application to .NET 5.</p>\r\n    <p>I'm going to stay with you.</p>\r\n    <h3 id=\"react\" style=\"position:relative;\"><a href=\"#react\" aria-label=\"react permalink\" class=\"anchor before\"><svg aria-hidden=\"true\" focusable=\"false\" height=\"16\" version=\"1.1\" viewbox=\"0 0 16 16\" width=\"16\">\r\n                <path fill-rule=\"evenodd\" d=\"M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z\">\r\n                </path>\r\n            </svg></a>React</h3>\r\n    <p>In 2020, he was responsible for web front-end work for almost most projects.</p>\r\n    <p>We've used React, and we're going to continue reacting in the future.</p>\r\n    <h3 id=\"nodejs\" style=\"position:relative;\"><a href=\"#nodejs\" aria-label=\"nodejs permalink\" class=\"anchor before\"><svg aria-hidden=\"true\" focusable=\"false\" height=\"16\" version=\"1.1\" viewbox=\"0 0 16 16\" width=\"16\">\r\n                <path fill-rule=\"evenodd\" d=\"M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z\">\r\n                </path>\r\n            </svg></a>Node.js</h3>\r\n    <p>Unless it's a big project, nodejs is thought of as a good backend platform.</p>\r\n    <p><a href=\"http://expressjs.com\">express.js,</a> and now <a href=\"http://nestjs.com/\">to take advantage of nestjs,</a> \r\n        I'm studying.</p>\r\n    <p>We also use it to create cross-platform CLI tools.</p>\r\n    <h3 id=\"typescript\" style=\"position:relative;\"><a href=\"#typescript\" aria-label=\"typescript permalink\" class=\"anchor before\"><svg aria-hidden=\"true\" focusable=\"false\" height=\"16\" version=\"1.1\" viewbox=\"0 0 16 16\" width=\"16\">\r\n                <path fill-rule=\"evenodd\" d=\"M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z\">\r\n                </path>\r\n            </svg></a>Typescript</h3>\r\n    <p>Code that needs to be written in JavaScript now feels more comfortable using Typescript.</p>\r\n    <p>It's inconvenient to write a little more code, and it's inconvenient to configure transfiles, but it's more convenient when writing code.</p>\r\n    <p>I'm thinking of continuing to use Typescript.</p>\r\n    <h3 id=\"container\" style=\"position:relative;\"><a href=\"#container\" aria-label=\"container permalink\" class=\"anchor before\"><svg aria-hidden=\"true\" focusable=\"false\" height=\"16\" version=\"1.1\" viewbox=\"0 0 16 16\" width=\"16\">\r\n                <path fill-rule=\"evenodd\" d=\"M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z\">\r\n                </path>\r\n            </svg></a>Container</h3>\r\n    <p>I'm learning container skills as a docker, and I need to learn other containers a little bit.</p>\r\n    <p>You have transferred a web application that operates on AWS Lightsail to use a container.</p>\r\n    <h3 id=\"react-native\" style=\"position:relative;\"><a href=\"#react-native\" aria-label=\"react native permalink\" class=\"anchor before\"><svg aria-hidden=\"true\" focusable=\"false\" height=\"16\" version=\"1.1\" viewbox=\"0 0 16 16\" width=\"16\">\r\n                <path fill-rule=\"evenodd\" d=\"M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z\">\r\n                </path>\r\n            </svg></a>React Native</h3>\r\n    <p>Looking back on 2019, there were things I wanted to do in 2020, and there's a change of mind.</p>\r\n    <p>I think it's a good technology when writing a mobile app that consists of simple features.</p>\r\n    <p>However, it was very difficult if you needed a feature or native feature that you couldn't implement yourself, and you searched for a package to use it and you couldn't find the code that was implemented.</p>\r\n    <p>The other platforms would be the same, but I thought it would be very difficult to access them easily.</p>\r\n    <p>Rather than taking it to the main, this technology should only be used when implementing a simple mobile app.</p>\r\n    <h2 id=\"2020년-이야기\" style=\"position:relative;\"><a href=\"#2020%EB%85%84-%EC%9D%B4%EC%95%BC%EA%B8%B0\" aria-label=\"2020년 이야기 permalink\" class=\"anchor before\"><svg aria-hidden=\"true\" focusable=\"false\" height=\"16\" version=\"1.1\" viewbox=\"0 0 16 16\" width=\"16\">\r\n                <path fill-rule=\"evenodd\" d=\"M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z\">\r\n
 </path>\r\n            </svg></a>2020 Stories</h2>\r\n    <p>We worked with the startup team.</p>\r\n    <p>Some projects have abandoned their projects because the team's service plans have not been organized, while some have not yet started service as a social statement.</p>\r\n    <p>And I started working part-time.</p>\r\n    <p>When the person planning the service organized the problem with a small startup team that was not organized, the planner continued to give the idea.\r\n        The service concept has continued to change.\r\n        After a while, the team abandoned the service because it seemed difficult to implement it.</p>\r\n    <p>Teams that have not yet started service in the final stages are being delayed from starting service due to COVID-19.</p>\r\n    <p>In addition, the team I contacted on Facebook Chat suggested part-time work.</p>\r\n    <h3 id=\"응용프로그램\" style=\"position:relative;\"><a href=\"#%EC%9D%91%EC%9A%A9%ED%94%84%EB%A1%9C%EA%B7%B8%EB%9E%A8\" aria-label=\"응용프로그램 permalink\" class=\"anchor before\"><svg aria-hidden=\"true\" focusable=\"false\" height=\"16\" version=\"1.1\" viewbox=\"0 0 16 16\" width=\"16\">\r\n                <path fill-rule=\"evenodd\" d=\"M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z\">\r\n                </path>\r\n            </svg></a>Applications</h3>\r\n    <p>A list of applications published in 2020.</p>\r\n    <ul>\r\n        <li><a href=\"https://flexbox.bbon.me/\">Help me! Display: flex</a></li>\r\n        <li><a href=\"https://guid.bbon.me/\">GUID Generator</a></li>\r\n        <li><a href=\"https://play.google.com/store/apps/details?id=kr.bbon.whendoibuyamask\">Google Play: Public Mask Guide</a></li>\r\n        <li><a href=\"https://www.npmjs.com/package/@bbon/formatter\">npm: @bbon/formatter</a></li>\r\n        <li><a href=\"https://www.npmjs.com/package/@bbon/filedownload\">npm: @bbon/filedownload</a></li>\r\n        <li><a href=\"https://www.npmjs.com/package/@bbon/css-to-jss\">npm: @bbon/css-to-jss</a></li>\r\n        <li><a href=\"https://resume.bbon.me/\">Public Resume</a></li>\r\n    </ul>\r\n    <h2 id=\"2021년에는\" style=\"position:relative;\"><a href=\"#2021%EB%85%84%EC%97%90%EB%8A%94\" aria-label=\"2021년에는 permalink\" class=\"anchor before\"><svg aria-hidden=\"true\" focusable=\"false\" height=\"16\" version=\"1.1\" viewbox=\"0 0 16 16\" width=\"16\">\r\n                <path fill-rule=\"evenodd\" d=\"M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z\">\r\n                </path>\r\n            </svg></a>In 2021,</h2>\r\n    <p>I want to find work as slowly as I do now, and I want to do it.</p>\r\n    <p>I want to keep up with the good people.</p>\r\n    <p>You must purchase a mac with Apple silicon.\r\n        We look forward to presenting more high-performance technology than M1.</p>\r\n    <p>We are waiting for that time to become safe from COVID-19.\r\n        There are many people I would like to meet, and I would like to see you.</p>\r\n</div>",
          "Transliteration": null,
          "To": "en",
          "Alignment": null,
          "SentLen": null
        }
      ]
    }
  ],
  "StatusCode": 200,
  "Message": ""
}
```