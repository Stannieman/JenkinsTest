pipeline {
  agent {
    label 'Git'
  }
  stages {
    stage('Build') {
      steps {
		powershell 'Invoke-Expression "$env:CAPABILITY_MSBUILD_15.7 /t:Build /p:Configuration=Release"'
      }
    }
	stage('Test') {
		powershell 'Write-Host "Teeest"'
	}
	stage('Pack') {
		powershell 'Invoke-Expression "$env:CAPABILITY_MSBUILD_15.7 /t:pack /p:Configuration=Release"'
	}
	stage('Publish') {
		echo "Publish"
	}
  }
}
