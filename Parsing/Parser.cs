﻿using MiniDown.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniDown.Parsing
{
    public sealed class Parser
    {
        public static List<IAstElement> ParseMinidown(string source)
        {
            Parser parser = new(source);
            return parser.ParseAll();
        }

        private readonly Scanner scanner;

        private Parser(string source)
        {
            scanner = new Scanner(source);
        }

        private void MatchToken(TokenType expectedToken)
        {
            if (scanner.LastToken.Type != expectedToken)
                throw new UnexpectedTokenException(scanner.LastToken.Type, expectedToken);
        }

        private void MatchAndScan(TokenType expectedToken)
        {
            MatchToken(expectedToken);
            scanner.ScanToken();
        }

        //parses one line as text block elements
        private List<ITextBlockElement> ParseLine(char? firstChar = null)
        {
            bool parsingInline = false;
            StringBuilder currentSnippet = new();
            List<ITextBlockElement> elements = new();

            if (firstChar != null)
                currentSnippet.Append(firstChar);

            while(true)
            {
                switch (scanner.LastToken.Type)
                {
                    case TokenType.OtherCharacters:
                    case TokenType.Space:
                    case TokenType.Bullet:
                        currentSnippet.Append(scanner.LastToken.Char);
                        break;
                    case TokenType.Backtick:
                        scanner.ScanToken();
                        if (parsingInline)
                            elements.Add(new CodeSnippet(currentSnippet.ToString()));
                        else
                            elements.Add(new TextSnippet(currentSnippet.ToString()));
                        currentSnippet.Clear();
                        parsingInline = !parsingInline;
                        break;
                    case TokenType.Newline:
                    case TokenType.Eof:
                        if (parsingInline)
                            throw new UnexpectedTokenException(scanner.LastToken.Type, TokenType.Backtick);
                        else if (currentSnippet.Length > 0)
                        {
                            scanner.ScanToken();
                            elements.Append(new TextSnippet(currentSnippet.ToString()));
                            currentSnippet.Clear();
                        }
                        return elements;
                    default:
                        throw new UnexpectedTokenException(scanner.LastToken.Type, null);
                }
            }
        }

        private Paragraph ParseParagraph(List<ITextBlockElement>? firstElements)
        {
            //scan whitespace
            while (scanner.LastToken.Type == TokenType.Space || scanner.LastToken.Type == TokenType.Tab)
                scanner.ScanToken();

            List<ITextBlockElement> elements = new();
            if (firstElements != null)
                elements.AddRange(firstElements);

            do
            {
                elements.AddRange(ParseLine());
            }
            while(scanner.LastToken.Type == TokenType.Newline);

            Debug.Assert(elements.Count > 0);
            return new Paragraph(elements);
        }

        private Heading ParseHeading()
        {
            MatchAndScan(TokenType.Hashtag);
            
            int level = 1;
            while(scanner.LastToken.Type == TokenType.Hashtag)
            {
                scanner.ScanToken();
                level++;
            }
            Debug.Assert(level <= 6);
            MatchAndScan(TokenType.Space);

            return new Heading(level, ParseLine());
        }

        //returns list of ast elements with a bulleted list (if parsing succeded), and maybe a paragraph where the bullet is interpreted literally
        private IAstElement ParseBulletedList()
        {
            List<BulletedList.BulletedItem> items = new();
            while(true)
            {
                MatchAndScan(TokenType.Bullet);
                if (scanner.LastToken.Type != TokenType.Space)
                    return ParseParagraph(ParseLine('*'));

                List<ITextBlockElement> itemElements = ParseLine();
                Debug.Assert(itemElements.Count > 0);
                
                while(true) //the second newline
                {
                    if (scanner.LastToken.Type == TokenType.Bullet)
                    {
                        items.Add(new(itemElements));
                        break;
                    }
                    else if (scanner.LastToken.Type == TokenType.Newline || scanner.LastToken.Type == TokenType.Eof) //two newlines in a row, or eof - stop parsing bulleted list
                    {
                        scanner.ScanToken();
                        items.Add(new(itemElements));
                        return new BulletedList(items);
                    }
                    else
                        itemElements.AddRange(ParseLine());
                }
            }
        }

        private CodeBlock ParseCodeBlock()
        {
            MatchAndScan(TokenType.Backtick);
            MatchAndScan(TokenType.Backtick);
            MatchAndScan(TokenType.Backtick);
            MatchAndScan(TokenType.Newline);

            List<string> lines = new();
            while (true) 
            {
                StringBuilder line = new();
                do
                {
                    line.Append(scanner.LastToken.Char);
                    scanner.ScanToken();
                }
                while (scanner.LastToken.Type == TokenType.Newline);
                lines.Add(line.ToString());

                if(scanner.LastToken.Type == TokenType.Backtick)
                {
                    MatchAndScan(TokenType.Backtick);
                    MatchAndScan(TokenType.Backtick);
                    MatchAndScan(TokenType.Backtick);
                    MatchAndScan(TokenType.Newline);
                    return new CodeBlock(lines);
                }
            }
        }

        public List<IAstElement> ParseAll()
        {
            List<IAstElement> elements = new();
            while(true)
            {
                switch (scanner.LastToken.Type)
                {
                    case TokenType.Hashtag:
                        elements.Add(ParseHeading());
                        break;
                    case TokenType.Backtick:
                        elements.Add(ParseCodeBlock());
                        break;
                    case TokenType.Bullet:
                        elements.Add(ParseBulletedList());
                        break;
                    case TokenType.Newline:
                        scanner.ScanToken();
                        continue;
                    case TokenType.Eof:
                        return elements;
                    default:
                        elements.Add(ParseParagraph(null));
                        break;
                }
            }
        }
    }
}