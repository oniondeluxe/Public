# How will a typical solution look like?

Let's assume for a while that we are in the process of creating a new domain model implementation for a very simple application: we want to work with CRUD of a **flat file of customers**. How would such an implementation be used, from the perspective of an application developer?  

This is an almost hello world like illustration, and can be rendered in thousands of different variants. But let's narrow this down by looking at the shape of the solution if if would adhere to how M10 works.

## Scope

The example below is made with the following pre conditions and assumptions:  
* The user of the code is the application developer. The domain model itself, and all the underlying plumbing, has alredy been done elsewhere.
* The example will focus on fundamental concepts around how that model is _used_, not how it's implemented under the hood.
* The model will serve as the presentation layer for a domain model that is consumed in a distributed environment, ie a multi user and shared data environment where updates can occur somewhere else on the network at any given moment.

## Building blocks in C#

TBA

## How to use the model

TBA


## A canonical description

All of the code above... Xaml

## Summary
