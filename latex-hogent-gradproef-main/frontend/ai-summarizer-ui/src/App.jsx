import React, { useState } from "react";
import {
  Container,
  Box,
  Typography,
  TextField,
  Button,
  Paper,
  CircularProgress,
  Alert
} from "@mui/material";

const API_URL = "http://localhost:5111/api/summarize";

export default function App() {
  const [file, setFile] = useState(null);
  const [fileName, setFileName] = useState("");

  const [emailText, setEmailText] = useState("");
  const [summary, setSummary] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [extracted, setExtracted] = useState(false);

  const handleFile = (e) => {
    const f = e.target.files[0];
    if (!f) return;

    setFile(f);
    setFileName(f.name);

    setEmailText("");
    setSummary("");
    setError("");
    setExtracted(false);
  };

  const extractMail = async () => {
    if (!file) return;

    setLoading(true);
    setError("");
    setEmailText("");

    try {
      const fd = new FormData();
      fd.append("file", file);

      const res = await fetch(API_URL, {
        method: "POST",
        body: fd
      });

      if (!res.ok) {
        const txt = await res.text();
        throw new Error(txt || "Serverfout bij extractie.");
      }

      const data = await res.json();

      if (!data.originalText) {
        setError("Geen tekst uit bestand ontvangen.");
        return;
      }

      setEmailText(data.originalText);
      setExtracted(true);
    } catch (err) {
      console.error(err);
      setError("Mail kon niet worden geëxtraheerd.");
    } finally {
      setLoading(false);
    }
  };

  const summarize = async () => {
    if (!emailText.trim()) return;

    setLoading(true);
    setError("");
    setSummary("");

    try {
      const res = await fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          EmailText: emailText
        })
      });

      if (!res.ok) {
        const txt = await res.text();
        throw new Error(txt || "Serverfout bij samenvatten.");
      }

      const data = await res.json();
      setSummary(data.summary || "Geen samenvatting ontvangen.");
    } catch (err) {
      console.error(err);
      setError("Fout bij het ophalen van de samenvatting.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="md">
      <Box sx={{ mt: 6 }}>
        <Typography variant="h4" gutterBottom>
          AI Support Mail Samenvatter
        </Typography>

        <Paper sx={{ p: 3, mb: 3 }}>
          <Typography variant="subtitle1" gutterBottom>
            Upload een e-mailbestand (.msg / .eml / .txt)
          </Typography>

          <Button variant="outlined" component="label">
            Bestand kiezen
            <input
              type="file"
              hidden
              accept=".msg,.eml,.txt"
              onChange={handleFile}
            />
          </Button>

          {fileName && (
            <Typography sx={{ mt: 2 }} variant="body2">
              <strong>Geselecteerd bestand:</strong> {fileName}
            </Typography>
          )}
        </Paper>

        {file && (
          <Paper sx={{ p: 3, mb: 3 }}>
            <Typography variant="subtitle1" gutterBottom>
              Stap 2: Mail extracten
            </Typography>

            <Button
              variant="contained"
              onClick={extractMail}
              disabled={loading}
            >
              {loading ? "Bezig..." : "Extract"}
            </Button>
          </Paper>
        )}

        {extracted && (
          <Paper sx={{ p: 3, mb: 3 }}>
            <Typography variant="subtitle1" gutterBottom>
              Stap 3: Geëxtraheerde inhoud
            </Typography>

            <TextField
              fullWidth
              multiline
              rows={10}
              margin="normal"
              label="Inhoud van de supportmail"
              value={emailText}
              onChange={(e) => setEmailText(e.target.value)}
            />
          </Paper>
        )}

        {extracted && (
          <Paper sx={{ p: 3, mb: 3 }}>
            <Typography variant="subtitle1" gutterBottom>
              Stap 4: Analyse
            </Typography>

            <Button
              variant="contained"
              onClick={summarize}
              disabled={loading || !emailText}
            >
              {loading ? "Bezig..." : "Samenvatten"}
            </Button>

            {loading && (
              <Box sx={{ mt: 2 }}>
                <CircularProgress size={24} />
              </Box>
            )}
          </Paper>
        )}

        {error && (
          <Box sx={{ mt: 2 }}>
            <Alert severity="error">{error}</Alert>
          </Box>
        )}

        {summary && (
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Samenvatting
            </Typography>
            <Typography variant="body1" sx={{ whiteSpace: "pre-line" }}>
              {summary}
            </Typography>
          </Paper>
        )}
      </Box>
    </Container>
  );
}
