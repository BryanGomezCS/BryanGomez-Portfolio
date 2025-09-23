//
//  TriviaView2.swift
//  TriviaGameDemo
//
//  Created by Bryan Gomez on 10/20/24.
//

import SwiftUI

struct TriviaView2: View {
@EnvironmentObject var triviaManager: TriviaMainManager
    var body: some View {
    if triviaManager.reachedEnd {
            VStack (spacing: 20){
                Text("Trivia Game")
                    .lilacTitle()
                    .padding(.bottom, 60)
                
                Text("Congratulations! You've completed the trivia game! 🥳")
                Text("You scored \(triviaManager.score) our of \(triviaManager.length)")
                    .fontWeight(.heavy)
                
               Button {
                Task.init {
                    await triviaManager.fetchTrivia()
                }
            }label: {
                PrimaryButton(text: "Play Again")
            }
            .padding(.top, 30)
                
        }
            .foregroundColor(Color ("AccentColor"))
            .padding ()
            .frame(maxWidth: .infinity, maxHeight: .infinity)
            .background(Color(hue: 0.601, saturation: 0.127, brightness: 0.277))
            
        } else {
            QuestionView()
                .environmentObject(triviaManager)
        }
        
    }
}

#Preview {
    TriviaView2()
        .environmentObject(TriviaMainManager())
}
