//  QuestionView.swift
//  TriviaGameDemo
//
//  Created by Bryan Gomez on 10/16/24.
//

import SwiftUI

struct QuestionView: View {
@EnvironmentObject var triviaManager: TriviaMainManager
    
    var body: some View {
        VStack(spacing: 40) {
            HStack {
                Text("Trivia Game")
                    .lilacTitle()
                
                Spacer()
                
                Text("\(triviaManager.index + 1) out of \(triviaManager.length)")
                    .foregroundColor(Color("AccentColor"))
                    .fontWeight(.heavy)
            }
            
            ProgressBar(progress: triviaManager.progress)
            
            VStack(alignment: .leading, spacing: 20) {
                Text(triviaManager.question)
                    .font(.system(size: 20))
                    .bold()
                    .foregroundColor(.white)
                
                
                ForEach(triviaManager.answerChoices, id: \.id) { answer in
                    AnswerRow(answer: answer)
                        .environmentObject(triviaManager)
                }
                
          if triviaManager.answerSelected && !triviaManager.correctAnswer.isEmpty {
            Text("Correct Answer: \(triviaManager.correctAnswer)")
                .foregroundColor(.white) // Color of correct answer
                .padding()
                .fontWeight(.heavy)
            }
        }
            
            Button {
                triviaManager.goToNextQuestion()
            } label: {
                PrimaryButton(text: "Next", background: triviaManager.answerSelected ? Color("AccentColor") : .gray)
            }
            .disabled(!triviaManager.answerSelected)
            
            
            Spacer()
            
        }
        .padding()
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color(hue: 0.601, saturation: 0.127, brightness: 0.277))
        .navigationBarHidden(true)
    }
}

#Preview {
    QuestionView()
        .environmentObject(TriviaMainManager())
}
