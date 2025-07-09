# AI-Powered Blog Post Generator

A Semantic Kernel-based application that leverages multiple AI agents to generate comprehensive, SEO-optimized blog posts with visual content suggestions.

## Overview

This project implements a modular blog post generation system using Microsoft's Semantic Kernel framework. It orchestrates multiple specialized AI agents to handle different aspects of blog post creation, from outline generation to image suggestions.

## Project Architecture

### AI Agents

- **Outline Agent**: Develops structured blog post templates with organized sections
- **Content Generation Agent**: Creates detailed content for each section based on the outline
- **Editing & Proofreading Agent**: Ensures correctness, style consistency, and coherence
- **SEO Optimization Agent**: Integrates target keywords and optimizes content for search engines
- **Visual Suggestions Agent**: Proposes relevant images and media placements
- **CTA & Engagement Agent**: Crafts compelling calls-to-action
- **Image Generation Agent**: Creates AI-generated visuals using DALL-E

### Orchestration Patterns

- **Sequential (Pipeline) Pattern**: Orchestrates the flow of content through different processing stages
- **Fan-Out/Fan-In Pattern**: Enables parallel processing where applicable
- **Chain-of-Thought Prompt Chaining**: Maintains context across different processing steps
- **Composite Orchestration**: Manages dependencies and result consolidation

## Technical Details

- Built with .NET 9
- Uses Microsoft Semantic Kernel for AI function orchestration
- Implements dependency injection for modularity and testability
- Includes comprehensive error handling and logging
- Supports parallel processing for improved performance

## Project Generation

- Initial project prompt was generated using OpenAI's o3-mini model
- Project implementation and architecture were built out using Claude Sonnet 3.5

## Requirements

- OpenAI API key (for GPT-4 and DALL-E integration)
- .NET 9.0 SDK
- Environment variable setup for API keys

## Features

- Complete blog post generation from a single topic input
- SEO optimization with keyword integration
- AI-generated image suggestions
- Customizable calls-to-action
- Modular architecture for easy extension
- Error handling and logging throughout the process
