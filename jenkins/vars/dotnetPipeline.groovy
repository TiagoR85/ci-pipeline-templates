// Shared-library entry point:
//   dotnetPipeline(solution: 'app.slnx', testProject: 'tests/T.csproj')
def call(Map config = [:]) {
    def solution = config.get('solution', 'solution.slnx')
    def testProject = config.get('testProject', 'tests/Tests.csproj')
    def coverageThreshold = config.get('coverageThreshold', '80')

    pipeline {
        agent any
        stages {
            stage('Build') {
                steps {
                    sh "dotnet build ${solution} -c Release"
                }
            }
            stage('Test') {
                steps {
                    sh "dotnet test ${testProject} -c Release -p:CollectCoverage=true -p:CoverletOutputFormat=cobertura -p:Threshold=${coverageThreshold} -p:ThresholdType=line"
                }
            }
        }
    }
}
