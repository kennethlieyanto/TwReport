{{/*
Chart label
*/}}
{{- define "twreport.name" -}}
{{- .Chart.Name | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "twreport.labels" -}}
helm.sh/chart: {{ .Chart.Name }}-{{ .Chart.Version | replace "+" "_" }}
{{ include "twreport.selectorLabels" . }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "twreport.selectorLabels" -}}
app.kubernetes.io/name: {{ include "twreport.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}
