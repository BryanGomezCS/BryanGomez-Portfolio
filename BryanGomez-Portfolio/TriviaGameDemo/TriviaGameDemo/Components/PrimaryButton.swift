//
//  PrimaryButton.swift
//  TriviaGameDemo
//
//  Created by Bryan Gomez on 10/16/24.
//

import SwiftUI

struct PrimaryButton: View {
    var text: String
    var background: Color = .green
    var body: some View {
        Text(text)
            .foregroundStyle(.white)
            .padding()
            .padding(.horizontal)
            .background(background)
            .cornerRadius(30)
            .shadow(radius: 10)
            
    }
}

#Preview {
    PrimaryButton(text: "Next")
}
