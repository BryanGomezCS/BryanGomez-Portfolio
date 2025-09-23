//
//  ContentView.swift
//  TriviaGameDemo
//
//  Created by Bryan Gomez on 10/16/24.
//

import SwiftUI

struct ContentView: View {
    @StateObject var triviaManager = TriviaMainManager()
    
    var body: some View {
        NavigationView {
            VStack(spacing: 40){
                VStack(spacing: 20) {
                    Text("Trivia Game")
                        .lilacTitle()
                    
                    Text("Welcome to the Trivia Game! Are you ready to test your knowledge?")
                        .font(.headline)
                        .fontWeight(.bold)
                        .foregroundColor(Color("AccentColor"))
                        .padding()
                    
                }
                NavigationLink {
                    TriviaView2()
                        .environmentObject(triviaManager)
                }
                label: {
                    PrimaryButton(text: "Start Trivia")
                        
                }
               
                
            }
            .frame(maxWidth:.infinity, maxHeight:.infinity)
            .edgesIgnoringSafeArea(.all)
            .background(Color(hue: 0.023, saturation: 0.0, brightness: 0.194))
        }
        
    }
}

#Preview {
    ContentView()
       
}
